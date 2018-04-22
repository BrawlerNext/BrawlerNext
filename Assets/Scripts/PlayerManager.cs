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
    public float ImpulseMultiplier = 100f;
    public float delayImpulseOnHit = 0.5f;

    public bool isInvulnerable = false;
    public float invulnerableTime = 2;

    // Basic input data
    public Character character;
    public Player player;
    public Controller controller;
    public PlayerManager playerManager;

    // Colliders of the player
    public bool isIgnoringForward = false;
    protected Collider leftPunchCollider;

    protected Collider dashCollider;

    protected Collider rightPunchCollider;
    protected GameObject shield;

    public float stunTime = 5f;


    //retjrejtopertjeroptjoetorep
    public GameObject stunEffect;
    public GameObject pushedEFFECT;


    // Stateless data
    protected Rigidbody rb;
    protected GroundChecker groundChecker;
    protected ControlManager controlManager;
    protected ParticleManager particleManager;
    protected AudioManager audioManager;

    protected Animator animator;

    protected Transform otherPlayer;

    // Runtime data
    protected bool isStunned = false;
    protected bool isPushed = false;
    protected int currentCombo = 1;
    protected int currentJumps = 0;

    protected float currentShieldLife = 0;
    protected bool shieldsUp = false;
    protected bool shieldIsRepairing = false;

    protected float impulseDelay = 0;
    protected float impulse = 0;
    protected float damage = 0;

    protected Vector3 lastMovementNormalized;
    protected float dashDelay = 999;

    protected Actions lastAction = Actions.IDLE;

    protected Dictionary<Actions, bool> Flags = new Dictionary<Actions, bool>();
    protected Dictionary<Actions, bool> ActuallyDoing = new Dictionary<Actions, bool>();

    protected Actions[] AllActions = new[]
        {Actions.JUMP, Actions.HARD_PUNCH, Actions.SOFT_PUNCH, Actions.MOVE, Actions.DEFEND, Actions.DASHING};

    void Awake()
    {
        particleManager = new ParticleManager(character);
        audioManager = new AudioManager(gameObject.GetComponent<AudioSource>(), character);

        rb = gameObject.GetComponent<Rigidbody>();
        groundChecker = gameObject.GetComponentInChildren<GroundChecker>();
        animator = gameObject.GetComponentInChildren<Animator>();

        controlManager = gameObject.GetComponent<ControlManager>();

        playerManager = gameObject.GetComponent<PlayerManager>();

        controlManager.Init(controller, player, playerManager);
        //rb.isKinematic = true;

        currentShieldLife = character.shieldLife;

        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("Shield"))
            {
                shield = child.gameObject;
                shield.SetActive(false);
            }

            if (child.CompareTag("LeftPunchCollider"))
            {
                leftPunchCollider = child.GetComponent<Collider>();
                leftPunchCollider.enabled = false;
            }

            if (child.CompareTag("RightPunchCollider"))
            {
                rightPunchCollider = child.GetComponent<Collider>();
                rightPunchCollider.enabled = false;
            }

            if (child.name == "DashCollider")
            {
                dashCollider = child.GetComponent<Collider>();
                dashCollider.enabled = false;
            }
        }

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
                otherPlayer = GameObject.Find(Player.P2.ToString()).transform;
                break;
            case Player.P2:
                otherPlayer = GameObject.Find(Player.P1.ToString()).transform;
                break;
        }
    }



    protected void FixedUpdate()
    {

        if (!shieldsUp)
        {
            animator.SetBool("IsRunning", Flags[Actions.MOVE]);

            if (Flags[Actions.MOVE] && !isPushed)
            {
                Move();
            }
        }

        // DEBUG
        if (GameDirector.DebugginGame)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.ClearDeveloperConsole();
            }

            if (Input.GetKey(KeyCode.Z))
            {
                if (CompareTag("P1"))
                {
                    DebugData();
                }
            }

            if (Input.GetKey(KeyCode.X))
            {
                if (CompareTag("P2"))
                {
                    DebugData();
                }
            }

            stunEffect.SetActive(isStunned);

        }
        /*********/

        if (!controlManager.IsCancelTargeting())
        {
            transform.LookAt(otherPlayer);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }

        if (impulse > 0)
        {

            Debug.Log("Impulsed " + impulseDelay + " " + impulse);
            impulseDelay -= Time.deltaTime;

            animator.SetBool("IsDamaged", true);

            if (impulseDelay <= 0)
            {
                Debug.Log("Apply impulse " + impulseDelay);
                ApplyImpulse();
                isStunned = false;
                animator.SetBool("IsDamaged", false);
            }
            else
            {
                isStunned = true;
            }
        }


        if (isPushed)
        {
            pushedEFFECT.SetActive(true);

            if (rb.velocity.magnitude < 0.01f)
            {
                isPushed = false;

                pushedEFFECT.SetActive(false);
            }
        }


        if (!isStunned)
        {
            dashDelay += Time.deltaTime;

            bool isDashing = dashDelay < character.dashTimeInSeconds;

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
                    if (currentShieldLife < (character.shieldLife * 0.25))
                    {
                        audioManager.Play(AudioType.SHIELD_DOWN);
                        shieldIsRepairing = true;
                        StartCoroutine(RepairShield());
                    }

                    shieldsUp = controlManager.IsDefending();
                }
                else
                {
                    shieldsUp = false;
                }

                bool shieldIsActive = shield.activeSelf;

                if (!shieldIsActive && shieldsUp)
                {
                    audioManager.Play(AudioType.SHIELD_UP);
                }

                shield.SetActive(shieldsUp);

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

            isDashing = ActuallyDoing[Actions.DASHING] |= controlManager.IsDashing();

            animator.SetBool("IsDashing", isDashing);

            if (isDashing)
            {
                dashCollider.enabled = isDashing;
                StartCoroutine(DisableDashAfterSeconds());

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

            ActuallyDoing[Actions.JUMP] |= controlManager.IsJumping();
            if (Flags[Actions.JUMP])
            {
                if (ActuallyDoing[Actions.JUMP])
                {
                    ActuallyDoing[Actions.JUMP] = false;

                    if (groundChecker.isGrounded || currentJumps < character.maxJumps)
                    {
                        currentJumps++;
                        if (currentJumps == 2)
                        {
                            rb.velocity = new Vector3(0, 0, 0);
                        }
                        audioManager.Play(AudioType.JUMP);
                        animator.SetBool("IsJumping", true);
                        rb.mass = 73;
                        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                        rb.AddForce(Vector3.up * character.jumpForce * 10, ForceMode.Impulse);
                        Debug.Log("Vector3 " + Vector3.up + "characterjumpforce " + character.jumpForce * 10 + "elforcemode " + ForceMode.Impulse);
                        Jump();
                    }

                    return;
                }

                if (groundChecker.isGrounded)
                {
                    currentJumps = 0;
                }

                animator.SetBool("IsJumping", false);
            }

            ActuallyDoing[Actions.SOFT_PUNCH] |= controlManager.IsSoftAttacking();

            animator.SetBool("IsSoftAttacking", ActuallyDoing[Actions.SOFT_PUNCH] && Flags[Actions.SOFT_PUNCH]);
            if (Flags[Actions.SOFT_PUNCH])
            {
                if (ActuallyDoing[Actions.SOFT_PUNCH])
                {
                    lastAction = Actions.SOFT_PUNCH;
                    ActuallyDoing[Actions.SOFT_PUNCH] = false;
                    SoftAttack();
                    return;
                }
            }

            ActuallyDoing[Actions.HARD_PUNCH] |= controlManager.IsHardAttacking();

            animator.SetBool("IsHardAttacking", ActuallyDoing[Actions.HARD_PUNCH]);
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

    private void DebugData()
    {
        print("Is stunned?: " + isStunned);
        print("Current jump: " + currentJumps);
        print("Is grounded?: " + groundChecker.isGrounded);
        print("Shield current life: " + currentShieldLife);
        print("Shield is repairing?: " + shieldIsRepairing);
        print("Current impulse: " + impulse);
    }

    private IEnumerator RepairShield()
    {
        yield return new WaitForSeconds(character.shieldRepairDelay);
        shieldIsRepairing = false;
    }

    public void ApplyImpulse()
    {
        // impulse += damage * ImpulseMultiplier;
        //rb.AddForce(transform.forward * -1 * impulse, ForceMode.Impulse);
        if (isInvulnerable == true) return;
        rb.velocity = transform.forward * -1f * impulse;
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

        if (Math.Abs(horizontalMovement) != 0f || Math.Abs(verticalMovement) != 0f)
        {
            Vector3 horizontalVector = Camera.main.transform.right * controlManager.GetHorizontalMovement();
            Vector3 verticalVector = getCameraForwardVector() * controlManager.GetVerticalMovement();
            movement += groundChecker.isGrounded ? horizontalVector : horizontalVector / 2;
            movement += groundChecker.isGrounded ? verticalVector : verticalVector / 2;
        }

        animator.SetBool("IsRunning", (Math.Abs(movement.x) > 0.5f || Math.Abs(movement.z) > 0.5f));

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
        // Fix down force
        rb.AddForce(0, Physics.gravity.y, 0);
    }

    private Vector3 getCameraForwardVector()
    {
        Vector3 middleNormalized = (Camera.main.transform.forward + Vector3.up).normalized;

        return (middleNormalized - (Vector3.Dot(middleNormalized, Vector3.up) * Vector3.up)).normalized;
    }

    void OnTriggerEnter(Collider collider)
    {
        Vector3 positionToInstantiate = getCollisionPoint();

        if (!collider.tag.Contains(otherPlayer.tag)) return;

        switch (lastAction)
        {
            case Actions.SOFT_PUNCH:
                if (positionToInstantiate == Vector3.zero) return;
                Hit(character.softPunchDamage, positionToInstantiate, ParticleType.SOFT_HIT, AudioType.SOFT_HIT, false);
                break;
            case Actions.HARD_PUNCH:
                if (positionToInstantiate == Vector3.zero) return;
                float damage = character.hardPunchDamage;

                if (UnityEngine.Random.value < character.criticChance)
                {
                    StartCoroutine(StopTime());
                    damage *= 2;
                }

                Hit(damage, positionToInstantiate, ParticleType.HARD_HIT, AudioType.HARD_HIT, true);
                break;

            case Actions.DASHING:
                Debug.Log("AitoR " + collider.name);

                PlayerManager otherPlayerManager = otherPlayer.GetComponent<PlayerManager>();

                otherPlayerManager.Stun();
                break;
        }

    }

    public void Stun(float sTime = 0)
    {
        if (isStunned) return;

        isStunned = true;
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
    private IEnumerator StopTime()
    {
        //Time.timeScale = 0;
        //yield return new WaitForSecondsRealtime(0.5f);
        //Time.timeScale = 1;
        yield return 0;
    }

    private void Hit(float impulse, Vector3 positionToInstantiate, ParticleType particleType, AudioType audioType, bool inmediateImpulse)
    {
        PlayerManager otherPlayerManager = otherPlayer.GetComponent<PlayerManager>();

        otherPlayerManager.AddImpulse(impulse * ImpulseMultiplier);

        audioManager.Play(audioType);

        particleManager.InstantiateParticle(particleType, positionToInstantiate);

        if (currentCombo > 3 || inmediateImpulse) otherPlayerManager.ApplyImpulse();

        lastAction = Actions.IDLE;
    }

    private Vector3 getCollisionPoint()
    {
        if (leftPunchCollider.enabled) return leftPunchCollider.transform.position;
        if (rightPunchCollider.enabled) return rightPunchCollider.transform.position;

        return Vector3.zero;
    }

    public void AddImpulse(float impulse)
    {
        if (!shieldsUp)
        {
            DisableAllFlags();
            impulseDelay = delayImpulseOnHit;
            this.impulse += impulse;
            this.damage += (impulse / ImpulseMultiplier) * 0.1f;
        }
        else
        {
            currentShieldLife -= impulse;
            // * character.shieldDamagedReducedPercentage / ImpulseMultiplier;
            //TO DO: Jesé, el escudo nunca llega a 0, cuando se rompe stunea 3 segundos al enemigo.
            if (currentShieldLife < 50)
            {
                Stun(3);
                shieldsUp = false;
                shieldIsRepairing = true;
                StartCoroutine(RepairShield());

                DisableAllFlags();

            }
            audioManager.Play(AudioType.SHIELD_HIT);
        }
    }

    public bool isDead()
    {

        return transform.position.y < -10;

    }

    public float GetDamage()
    {
        return damage;
    }

    public void Reset()
    {
        damage = 0;
        impulse = 0;
        impulseDelay = 0;
        isPushed = false;
        rb.isKinematic = true;
        transform.position = GameObject.FindGameObjectWithTag("Respawn" + player).transform.position;
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
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

    public void toogleLeftPunchCollider()
    {
        leftPunchCollider.enabled = !leftPunchCollider.enabled;
    }
    public void toogleRightPunchCollider()
    {
        rightPunchCollider.enabled = !rightPunchCollider.enabled;
    }
    public void OnCollisionStay(Collision other)
    {
        if ((other.gameObject.tag == "P1" && player == Player.P2) ||
            (other.gameObject.tag == "P2" && player == Player.P1))
        {
            isIgnoringForward = true;
            Debug.Log("aaee");
        }
    }

    public void OnCollisionExit(Collision other)
    {
        if ((other.gameObject.tag == "P1" && player == Player.P2) ||
            (other.gameObject.tag == "P2" && player == Player.P1))
        {
            isIgnoringForward = false;
        }
    }


    IEnumerator DisableDashAfterSeconds()
    {
        yield return new WaitForSecondsRealtime(2);
        dashCollider.enabled = false;
    }
    public enum Actions
    {
        JUMP,
        SOFT_PUNCH,
        HARD_PUNCH,
        MOVE,
        DEFEND,
        IDLE,
        DASHING
    }
}