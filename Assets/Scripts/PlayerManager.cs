using System;
using System.Collections;
//using System.Collections.Concurrent;
using System.Collections.Generic;
using characters.scriptables;
using util;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public abstract class PlayerManager : MonoBehaviour
{
    // General modifiers
    public float impulseMultiplier = 0.1f;
    public float delayImpulseOnHit = 0.5f;

    public bool isInvulnerable = false;
    public float invulnerableTime = 2;

    // Basic input data
    public Character character;
    public Player player;
    public Controller controller;

    public float stunTime = 1f;

    public GameObject stunEffect;
    public GameObject pushedEFFECT;


    // Stateless data
    protected Rigidbody rb;
    protected GroundChecker groundChecker;
    protected ControlManager controlManager;
    protected ParticleManager particleManager;

    [HideInInspector]
    public AudioManager audioManager;
    protected CollisionManager cm;

    protected Animator animator;

    [HideInInspector]
    public Transform otherPlayer;

    // Runtime data
    protected bool isStunned = false;
    protected bool isPushed = false;
    protected bool isFreeze = false;
    protected bool isDashing = false;

    [HideInInspector]
    public int currentCombo = 1;
    protected int currentJumps = 0;

    protected float currentShieldLife = 0;

    [HideInInspector]
    public bool shieldsUp = false;
    protected bool shieldIsRepairing = false;

    protected float impulseDelay = 0;
    protected float impulse = 0;

    [HideInInspector]
    public float damage = 0;

    protected float aeroStopTime = 0;
    protected Vector3 startPosition = Vector3.zero;
    protected Vector3 endPosition = Vector3.zero;
    protected Vector3 direction = Vector3.zero;

    protected Vector3 lastMovementNormalized;

    protected IEnumerator finishingCombo = null;

    [HideInInspector]
    public float dashDelay = 999;

    [HideInInspector]
    public Actions lastAction = Actions.IDLE;
    protected Dictionary<Actions, bool> Flags = new Dictionary<Actions, bool>();
    protected Dictionary<Actions, bool> ActuallyDoing = new Dictionary<Actions, bool>();

    protected Actions[] AllActions = new[]
        {Actions.JUMP, Actions.HARD_PUNCH, Actions.SOFT_PUNCH, Actions.MOVE, Actions.DEFEND, Actions.DASHING, Actions.AERO_HIT};

    void Awake()
    {
        particleManager = new ParticleManager(character);
        audioManager = new AudioManager(gameObject.GetComponent<AudioSource>(), character);
        cm = GetComponent<CollisionManager>();

        rb = gameObject.GetComponent<Rigidbody>();
        groundChecker = gameObject.GetComponentInChildren<GroundChecker>();
        groundChecker.Init(character, audioManager);
        animator = gameObject.GetComponentInChildren<Animator>();

        controlManager = gameObject.GetComponent<ControlManager>();

        controlManager.Init(controller, player);

        currentShieldLife = character.shieldLife;

        SetFlagsTo(true);
        SetActuallyDoingTo(false);

        FindOtherPlayer();
    }

    private void SetFlagsTo(bool enabled)
    {
        foreach (Actions action in AllActions)
        {
            Flags[action] = enabled;
        }
    }

    private void SetActuallyDoingTo(bool enabled)
    {
        foreach (Actions action in AllActions)
        {
            ActuallyDoing[action] = enabled;
        }
    }

    private void FindOtherPlayer()
    {
        switch (player)
        {
            case Player.P1:
                otherPlayer = GameObject.FindGameObjectWithTag(Player.P2.ToString()).transform;
                break;
            case Player.P2:
                otherPlayer = GameObject.FindGameObjectWithTag(Player.P1.ToString()).transform;
                break;
        }
    }

    protected void Update()
    {
        if (isFreeze) return;

        if (!controlManager.IsCancelTargeting())
        {
            transform.LookAt(otherPlayer);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }

        if (impulse > 0)
        {

            impulseDelay -= Time.deltaTime;

            animator.SetBool("IsDamaged", true);

            if (impulseDelay <= 0)
            {
                ApplyImpulse();
                isStunned = false;
                animator.SetBool("IsDamaged", false);
            }
            else
            {
                isStunned = true;
            }
        }


        animator.SetBool("IsPushed", isPushed);
        if (isPushed)
        {
            pushedEFFECT.SetActive(true);

            if (rb.velocity.magnitude < 0.01f)
            {
                isPushed = false;

                pushedEFFECT.SetActive(false);
            }
        }

        stunEffect.SetActive(isStunned);
        animator.SetBool("IsDamaged", isStunned);

        if (!isStunned)
        {
            dashDelay += Time.deltaTime;

            bool wasDashing = isDashing;

            isDashing = dashDelay < character.dashTimeInSeconds;

            animator.SetBool("IsDashing", isDashing);

            if (wasDashing && !isDashing) lastAction = Actions.IDLE;

            if (isDashing)
            {
                Dash();
                return;
            }

            animator.SetInteger("Combo", currentCombo);
            animator.SetBool("InAir", !groundChecker.isGrounded);

            if (Flags[Actions.DEFEND] || !groundChecker.isGrounded)
            {
                if (!shieldIsRepairing)
                {
                    shieldsUp = controlManager.IsDefending();

                    if (shieldsUp && currentShieldLife < (character.shieldLife * 0.25))
                    {
                        Stun(3);
                        audioManager.Play(AudioType.SHIELD_DOWN);
                        shieldIsRepairing = true;
                        shieldsUp = false;
                        cm.shield.SetActive(false);
                        animator.SetBool("IsDefending", false);
                        StartCoroutine(RepairShield());
                        return;
                    }
                }
                else
                {
                    shieldsUp = false;
                }

                bool shieldIsActive = cm.shield.activeSelf;

                animator.SetBool("IsDefending", shieldIsActive);

                if (!shieldIsActive && shieldsUp)
                {
                    audioManager.Play(AudioType.SHIELD_UP);
                }

                cm.shield.SetActive(shieldsUp);

                if (shieldsUp)
                {
                    currentShieldLife -= Time.deltaTime * character.shieldDecayVelocity;
                    Defend();
                    return;
                }
                else
                {
                    currentShieldLife += Time.deltaTime * character.shieldRepairVelocity;

                    currentShieldLife = Math.Min(currentShieldLife, character.shieldLife);
                }
            }

            animator.SetBool("IsAeroAttacking", ActuallyDoing[Actions.AERO_HIT]);

            if (ActuallyDoing[Actions.AERO_HIT])
            {
                rb.velocity = Vector3.zero;

                if (aeroStopTime > 0)
                {
                    aeroStopTime -= Time.deltaTime;
                    transform.position += new Vector3(0, character.aeroAscensionDistance, 0);
                    return;
                }

                rb.velocity = direction * character.aeroPunchSpeed * 2;

                if (Vector3.Distance(startPosition, transform.position) > character.aeroMaxDistance)
                {
                    ActuallyDoing[Actions.AERO_HIT] = false;
                    aeroStopTime = 0;
                    EnableAllFlags();
                }

                bool collision = cm.CheckCollision();

                if (collision)
                {
                    ActuallyDoing[Actions.AERO_HIT] = false;
                    aeroStopTime = 0;
                    StartCoroutine(EnableAllFlagsAfterTime(0.5f));
                    return;
                }

                return;
            }

            isDashing = ActuallyDoing[Actions.DASHING] |= controlManager.IsDashing();

            if (isDashing)
            {
                cm.dashCollider.enabled = isDashing;
                StartCoroutine(cm.DisableDashAfterSeconds());
            }

            if (Flags[Actions.DASHING])
            {
                if (ActuallyDoing[Actions.DASHING])
                {
                    lastAction = Actions.DASHING;
                    ActuallyDoing[Actions.DASHING] = false;
                    dashDelay = 0;
                    DisableAllFlags();
                    StartCoroutine(EnableAllFlagsAfterTime(character.dashTimeInSeconds));
                    audioManager.Play(AudioType.DASH);
                    Dash();
                    return;
                }
            }

            ActuallyDoing[Actions.SOFT_PUNCH] |= controlManager.IsSoftAttacking();

            animator.SetBool("IsSoftAttacking", ActuallyDoing[Actions.SOFT_PUNCH] && Flags[Actions.SOFT_PUNCH] && groundChecker.isGrounded);
            if (Flags[Actions.SOFT_PUNCH])
            {
                if (ActuallyDoing[Actions.SOFT_PUNCH])
                {
                    if (groundChecker.isGrounded)
                    {
                        lastAction = Actions.SOFT_PUNCH;
                        ActuallyDoing[Actions.SOFT_PUNCH] = false;
                        SoftAttack();

                        if (finishingCombo != null) StopCoroutine(finishingCombo);

                        finishingCombo = FinishCombo();
                        StartCoroutine(finishingCombo);
                    }
                    else
                    {
                        if (Flags[Actions.AERO_HIT])
                        {
                            lastAction = Actions.AERO_HIT;
                            ActuallyDoing[Actions.SOFT_PUNCH] = false;

                            DisableAllFlags();
                            ActuallyDoing[Actions.AERO_HIT] = true;

                            startPosition = transform.position;
                            endPosition = otherPlayer.transform.position;
                            direction = (endPosition - startPosition).normalized;
                            aeroStopTime = character.aeroStopTime;
                        }
                    }
                    return;
                }
            }

            ActuallyDoing[Actions.HARD_PUNCH] |= controlManager.IsHardAttacking();

            animator.SetBool("IsHardAttacking", ActuallyDoing[Actions.HARD_PUNCH] && Flags[Actions.HARD_PUNCH]);
            if (Flags[Actions.HARD_PUNCH])
            {
                if (ActuallyDoing[Actions.HARD_PUNCH])
                {
                    lastAction = Actions.HARD_PUNCH;
                    ActuallyDoing[Actions.HARD_PUNCH] = false;
                    HardAttack();
                    return;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isFreeze) return;

        if (!shieldsUp && !isDashing)
        {
            animator.SetBool("IsRunning", Flags[Actions.MOVE]);

            if (Flags[Actions.MOVE] && !isPushed)
            {
                Move();
            }
        }

        if (impulse > 0)
        {
            return;
        }

        if (isPushed)
        {
            return;
        }

        if (!isStunned)
        {
            ActuallyDoing[Actions.JUMP] |= controlManager.IsJumping();

            if (Flags[Actions.JUMP])
            {
                if (ActuallyDoing[Actions.JUMP])
                {
                    ActuallyDoing[Actions.JUMP] = false;

                    if (groundChecker.isGrounded || currentJumps < character.maxJumps)
                    {
                        animator.SetBool("IsJumping", true);
                        audioManager.Play(AudioType.JUMP);
                        rb.mass = 73;
                        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                        rb.AddForce(Vector3.up * character.jumpForce * 10, ForceMode.Impulse);
                        Jump();
                        currentJumps++;
                    }

                    return;
                }

                if (groundChecker.isGrounded)
                {
                    currentJumps = 0;
                }

                animator.SetBool("IsJumping", false);
            }
        }
    }

    private IEnumerator FinishCombo()
    {
        yield return new WaitForSeconds(0.25f);
        ResetCombo();
        EnableAllFlags();
    }

    private IEnumerator RepairShield()
    {
        yield return new WaitForSeconds(character.shieldRepairDelay);
        shieldIsRepairing = false;
    }

    public void ApplyImpulse()
    {
        if (isInvulnerable == true) return;

        float totalImpulse = (impulse + (damage / 3));
        print("Impulse: " + totalImpulse);

        rb.velocity = transform.forward * -1f * totalImpulse;
        isPushed = true;
        animator.SetBool("IsDamaged", false);
        impulseDelay = 0;
        impulse = 0;
        isStunned = false;

        EnableAllFlags();
        SetActuallyDoingTo(false);
        ResetCombo();
    }

    private void Move()
    {
        Vector3 movement = Vector3.zero;

        float horizontalMovement = controlManager.GetHorizontalMovement();
        float verticalMovement = controlManager.GetVerticalMovement();

        animator.SetBool("IsRunning", false);

        if (Math.Abs(horizontalMovement) != 0f || Math.Abs(verticalMovement) != 0f)
        {
            Vector3 horizontalVector = Camera.main.transform.right * controlManager.GetHorizontalMovement();
            Vector3 verticalVector = getCameraForwardVector() * controlManager.GetVerticalMovement();
            movement += groundChecker.isGrounded ? horizontalVector : horizontalVector / 2;
            movement += groundChecker.isGrounded ? verticalVector : verticalVector / 2;
            lastAction = Actions.MOVE;

            audioManager.PlayOnce(AudioType.RUN);

            animator.SetBool("IsRunning", (Math.Abs(movement.x) > 0.5f || Math.Abs(movement.z) > 0.5f));
        }

        lastMovementNormalized = movement.normalized;

        movement *= character.speed;

        //Movement
        gameObject.transform.position = new Vector3
        (gameObject.transform.position.x + movement.x * character.speed * Time.deltaTime,
       gameObject.transform.position.y + movement.y * character.speed * Time.deltaTime,
       gameObject.transform.position.z + movement.z * character.speed * Time.deltaTime);


        if (movement.magnitude == 0)
            rb.mass = 144;
        else
            rb.mass = 73;

        if (!ActuallyDoing[Actions.AERO_HIT])
        {
            rb.AddForce(0, Physics.gravity.y / 5, 0);
        }
    }

    private Vector3 getCameraForwardVector()
    {
        Vector3 middleNormalized = (Camera.main.transform.forward + Vector3.up).normalized;

        return (middleNormalized - (Vector3.Dot(middleNormalized, Vector3.up) * Vector3.up)).normalized;
    }

    public void Stun(float sTime = 0)
    {
        if (isStunned) return;

        isStunned = true;
        DisableAllFlags();
        if (sTime == 0)
            StartCoroutine(DisableStun(stunTime));
        else
            StartCoroutine(DisableStun(sTime));
    }

    IEnumerator DisableStun(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        isStunned = false;
        EnableAllFlags();
    }

    public void AddImpulse(float impulse)
    {
        if (!shieldsUp)
        {
            DisableAllFlags();
            impulseDelay = delayImpulseOnHit;
            this.impulse += impulse;
            this.damage += (impulse / impulseMultiplier) * 0.1f;
        }
        else
        {
            currentShieldLife -= impulse;
            if (currentShieldLife < 50)
            {
                Stun(3);
                shieldsUp = false;
                shieldIsRepairing = true;
                animator.SetBool("IsDefending", false);
                StartCoroutine(RepairShield());
            }
            audioManager.Play(AudioType.SHIELD_HIT);
        }
    }

    public bool isDead()
    {

        return transform.position.y < -10;

    }

    // Reset all except the damage
    public void BasicReset()
    {
        impulse = 0;
        impulseDelay = 0;
        isPushed = false;
        rb.isKinematic = true;
        transform.position = GameObject.FindGameObjectWithTag("Respawn" + player).transform.position;
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
        lastAction = Actions.IDLE;
        StartCoroutine(InvulnerabilityAfterRespawn());
    }

    // Overrideable methods

    IEnumerator InvulnerabilityAfterRespawn()
    {
        isInvulnerable = true;
        yield return new WaitForSecondsRealtime(invulnerableTime);
        isInvulnerable = false;
    }


    public abstract void SoftAttack();
    public abstract void HardAttack();
    public abstract void Jump();
    public abstract void Defend();
    public abstract void Burn();

    public void Dash()
    {
        if (lastMovementNormalized == Vector3.zero)
            lastMovementNormalized = transform.forward;
        animator.SetBool("IsDashing", true);

        rb.velocity = lastMovementNormalized * character.dashSpeed * character.dashCurve.Evaluate(dashDelay);
    }

    public void Freeze(bool freeze)
    {
        isFreeze = freeze;
    }

    /*********************/

    // Animation keyframe methods

    public void EnableAllFlags()
    {
        SetFlagsTo(true);
    }

    private IEnumerator EnableAllFlagsAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        EnableAllFlags();
    }

    public void DisableAllFlags()
    {
        SetFlagsTo(false);
        SetActuallyDoingTo(false);
    }

    public void IncreaseCombo()
    {
        currentCombo++;
    }

    public void ResetCombo()
    {
        currentCombo = 1;
    }

    public void PlaySoundOf(AudioType type)
    {
        audioManager.Play(type);
    }

    public void toogleFlagOf(Actions action)
    {
        Flags[action] = !Flags[action];
    }

    public void setCurrentlyActionToFalse(Actions action)
    {
        ActuallyDoing[action] = false;
    }


}