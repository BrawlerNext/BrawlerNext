using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using characters.scriptables;
using util;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public abstract class PlayerManager : MonoBehaviour
{
    // General modifiers
    public float ImpulseMultiplier = 100f;

    // Basic input data
    public Character character;
    public Player player;
    public Controller controller;


    // Colliders of the player
    protected Collider leftPunchCollider;
    protected Collider rightPunchCollider;
    protected GameObject shield;


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
    protected int currentCombo = 1;
    protected int currentJumps = 0;

    protected float currentShieldLife = 0;
    protected bool shieldsUp = false;
    protected bool shieldIsRepairing = false;

    protected float impulseDelay = 0;
    protected float impulse = 0;

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
        controlManager.Init(controller, player);

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
                otherPlayer = GameObject.FindGameObjectWithTag(Player.P2.ToString()).transform;
                break;
            case Player.P2:
                otherPlayer = GameObject.FindGameObjectWithTag(Player.P1.ToString()).transform;
                break;
        }
    }

    protected void FixedUpdate()
    {
        if (!shieldsUp)
        {
            animator.SetBool("IsRunning", Flags[Actions.MOVE]);

            if (Flags[Actions.MOVE])
            {
                Move();
            }
        }
    }

    protected void Update()
    {
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
        }
        /*********/

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

            ActuallyDoing[Actions.DASHING] |= controlManager.IsDashing();

            animator.SetBool("IsDashing", ActuallyDoing[Actions.DASHING]);
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
                        audioManager.Play(AudioType.JUMP);
                        animator.SetBool("IsJumping", true);
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

            animator.SetBool("IsSoftAttacking", ActuallyDoing[Actions.SOFT_PUNCH]);
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
        rb.AddForce(transform.forward * -1 * impulse, ForceMode.Impulse);
        impulse = 0;
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
            movement += Camera.main.transform.right * controlManager.GetHorizontalMovement();
            movement += getCameraForwardVector() * controlManager.GetVerticalMovement();
        }

        animator.SetBool("IsRunning", (Math.Abs(movement.x) > 0.5f || Math.Abs(movement.z) > 0.5f));

        lastMovementNormalized = movement.normalized;

        movement *= character.speed;

        rb.velocity += movement;

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 15);
    }

    private Vector3 getCameraForwardVector()
    {
        Vector3 middleNormalized = (Camera.main.transform.forward + Vector3.up).normalized;

        return (middleNormalized - (Vector3.Dot(middleNormalized, Vector3.up) * Vector3.up)).normalized;
    }

    void OnCollisionEnter(Collision collision)
    {
        float impulse = 0;

        AudioType audio = AudioType.NONE;
        ParticleType particleType = ParticleType.NONE;
        Vector3 positionToInstantiate = Vector3.zero;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.thisCollider.tag.Contains("PunchCollider"))
            {
                switch (lastAction)
                {
                    case Actions.SOFT_PUNCH:
                        impulse = character.softPunchDamage;
                        particleType = ParticleType.SOFT_HIT;
                        positionToInstantiate = contact.point;
                        audio = AudioType.SOFT_HIT;
                        break;
                    case Actions.HARD_PUNCH:
                        impulse = character.hardPunchDamage;
                        particleType = ParticleType.HARD_HIT;
                        positionToInstantiate = contact.point;
                        audio = AudioType.HARD_HIT;
                        break;
                }
            }
        }

        if (impulse > 0)
        {
            otherPlayer.GetComponent<PlayerManager>().AddImpulse(impulse * ImpulseMultiplier);

            audioManager.Play(audio);

            particleManager.InstantiateParticle(particleType, positionToInstantiate);
        }
    }

    public void AddImpulse(float impulse)
    {
        if (!shieldsUp)
        {
            DisableAllFlags();
            impulseDelay = 1;
            this.impulse += impulse;
        }
        else
        {
            currentShieldLife -= impulse * character.shieldDamagedReducedPercentage / ImpulseMultiplier;
            audioManager.Play(AudioType.SHIELD_HIT);
        }
    }


    // Overrideable methods

    public abstract void SoftAttack();
    public abstract void HardAttack();
    public abstract void Jump();
    public abstract void Defend();
    public abstract void Burn();

    public void Dash()
    {
        if (lastMovementNormalized == Vector3.zero)
            lastMovementNormalized = transform.forward;
        
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

    public void toogleLeftPunchCollider()
    {
        leftPunchCollider.enabled = !leftPunchCollider.enabled;
    }

    public void toogleRightPunchCollider()
    {
        rightPunchCollider.enabled = !rightPunchCollider.enabled;
    }
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