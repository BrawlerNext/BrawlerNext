using System;
using System.Collections;
using System.Collections.Generic;
using characters.scriptables;
using util;
using UnityEngine;

public abstract class MovementManager : MonoBehaviour
{
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
    protected Animator animator;

    protected Transform otherPlayer;

    // Runtime data
    protected int currentCombo = 1;
    protected int currentJumps = 0;

    protected float currentShieldLife = 0;
    protected bool shieldsUp = false;
    protected bool shieldIsRepairing = false;

    protected float impulseDelay = 0;
    protected float impulse = 0;

    protected Dictionary<Actions, bool> Flags = new Dictionary<Actions, bool>();
    protected Dictionary<Actions, bool> ActuallyDoing = new Dictionary<Actions, bool>();

    protected Actions[] AllActions = new[]
        {Actions.JUMP, Actions.HARD_PUNCH, Actions.SOFT_PUNCH, Actions.MOVE, Actions.DEFEND};

    void Awake()
    {
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
                animator.SetBool("IsDamaged", false);
            }

            return;
        }

        animator.SetInteger("Combo", currentCombo);
        animator.SetBool("InAir", !groundChecker.isGrounded);

        if (Flags[Actions.DEFEND])
        {
            if (!shieldIsRepairing)
            {
                if (currentShieldLife < (character.shieldLife * 0.25))
                {
                    shieldIsRepairing = true;
                    StartCoroutine(RepairShield());
                }

                shieldsUp = controlManager.IsDefending();
            }
            else
            {
                shieldsUp = false;
            }

            shield.SetActive(shieldsUp);

            if (shieldsUp)
            {
                currentShieldLife -= Time.deltaTime * 10;

                return;
            }
            else
            {
                currentShieldLife += Time.deltaTime * 5;

                currentShieldLife = Math.Min(currentShieldLife, character.shieldLife);
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
                    animator.SetBool("IsJumping", true);
                    Jump();
                    currentJumps++;
                }

                return;
            }

            animator.SetBool("IsJumping", false);
        }

        ActuallyDoing[Actions.SOFT_PUNCH] |= controlManager.IsSoftAttacking();

        animator.SetBool("IsSoftAttacking", ActuallyDoing[Actions.SOFT_PUNCH]);
        if (Flags[Actions.SOFT_PUNCH])
        {
            if (ActuallyDoing[Actions.SOFT_PUNCH])
            {
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
                ActuallyDoing[Actions.HARD_PUNCH] = false;
                HardAttack();
                return;
            }
        }
    }

    private IEnumerator RepairShield()
    {
        yield return new WaitForSeconds(character.shieldRepairDelay);
        shieldIsRepairing = false;
    }

    public void EnableAllFlags()
    {
        SetFlagsTo(true);
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

    public void ApplyImpulse()
    {
        rb.AddForce(transform.forward * -1 * impulse, ForceMode.Impulse);
        impulse = 0;
        EnableAllFlags();
        SetActuallyDoingTo(false);
        ResetCombo();
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

    protected void Move()
    {
        Vector3 movement = Vector3.zero;

        float horizontalMovement = controlManager.GetHorizontalMovement();
        float verticalMovement = controlManager.GetVerticalMovement();

        if (Math.Abs(horizontalMovement) != 0f || Math.Abs(verticalMovement) != 0f)
        {
            movement += otherPlayer.forward * controlManager.GetHorizontalMovement() * -1;
            movement += otherPlayer.right * controlManager.GetVerticalMovement();
        }

        animator.SetBool("IsRunning", (Math.Abs(movement.x) > 0.5f || Math.Abs(movement.z) > 0.5f));

        movement *= character.speed;

        if (groundChecker.isGrounded)
        {
            currentJumps = 0;
        }

        rb.velocity += movement;

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 15);
    }

    void OnCollisionEnter(Collision collision)
    {
        float impulse = 0;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.thisCollider.tag.Contains("PunchCollider"))
            {
                if (animator.GetBool("IsSoftAttacking"))
                {
                    impulse = character.softPunchDamage;
                }
                else
                {
                    impulse = character.hardPunchDamage;
                }
            }
        }

        if (impulse > 0)
        {
            otherPlayer.GetComponent<MovementManager>().AddImpulse(impulse * 100f);
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
    }

    public abstract void SoftAttack();
    public abstract void HardAttack();
    public abstract void Jump();
    public abstract void Defend();
    public abstract void Dash();
    public abstract void Burn();
}

public enum Actions
{
    ALL,
    JUMP,
    SOFT_PUNCH,
    HARD_PUNCH,
    MOVE,
    DEFEND
}