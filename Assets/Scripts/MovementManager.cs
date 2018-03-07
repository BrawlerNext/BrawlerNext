using System;
using System.Collections;
using System.Collections.Generic;
using characters.scriptables;
using util;
using UnityEngine;

public abstract class MovementManager : MonoBehaviour
{
    public Character character;
    public Player player;
    public Controller controller;

    protected Collider leftPunchCollider;
    protected Collider rightPunchCollider;

    protected int currentCombo = 1;
    protected float impulse = 0;

    protected Rigidbody rb;
    protected GroundChecker groundChecker;
    protected ControlManager controlManager;
    protected Animator animator;

    protected Transform otherPlayer;

    protected bool inAnimation = false;
    protected int jumps = 0;

    protected bool isSoftAttacking = false;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        groundChecker = gameObject.GetComponentInChildren<GroundChecker>();
        animator = gameObject.GetComponentInChildren<Animator>();

        switch (player)
        {
            case Player.P1:
                otherPlayer = GameObject.FindGameObjectWithTag(Player.P2.ToString()).transform;
                break;
            case Player.P2:
                otherPlayer = GameObject.FindGameObjectWithTag(Player.P1.ToString()).transform;
                break;
        }

        Collider[] allColiders = gameObject.GetComponentsInChildren<Collider>();

        for (int i = 0; i < allColiders.Length; i++)
		{

            switch (allColiders[i].tag)
            {
                case "LeftPunchCollider":
                    leftPunchCollider = allColiders[i];
                    leftPunchCollider.enabled = false;
                    break;
                case "RightPunchCollider":
                    rightPunchCollider = allColiders[i];
                    rightPunchCollider.enabled = false;
                    break;
                default:
                    break;
            }
            	 
		}

        controlManager = gameObject.GetComponent<ControlManager>();
        controlManager.Init(controller, player);
    }

    protected void FixedUpdate()
    {
        Move();
    }

    protected void Update()
    {

        animator.SetInteger("Combo", currentCombo);

        transform.LookAt(otherPlayer);

        isSoftAttacking |= controlManager.IsSoftAttacking();

        animator.SetBool("IsSoftAttacking", isSoftAttacking);
        if (!inAnimation)
        {
            animator.SetBool("InAir", !groundChecker.isGrounded);

            if (controlManager.IsJumping())
            {
                if (groundChecker.isGrounded || jumps < character.maxJumps ||
                    (jumps == character.maxJumps && controlManager.IsBurning()))
                {
                    animator.SetBool("IsJumping", true);
                    Jump();
                    jumps++;
                }
                return;
            }
            animator.SetBool("IsJumping", false);

          
            if (isSoftAttacking)
            {
                isSoftAttacking = false;
                currentCombo++;
                animator.SetBool("IsSoftAttacking", true);
                SoftAttack();
                return;
            }
            

            if (controlManager.IsHardAttacking())
            {
                currentCombo++;
                animator.SetBool("IsHardAttacking", true);
                HardAttack();
                return;
            }
            animator.SetBool("IsHardAttacking", false);

            if (controlManager.IsDefending())
            {
                animator.SetBool("IsDefending", true);
                Defend();
                return;
            }
            animator.SetBool("IsDefending", false);

            if (controlManager.IsDashing())
            {
                animator.SetBool("IsDashing", true);
                Dash();
                return;
            }
            animator.SetBool("IsDashing", false);

            if (controlManager.IsBurning())
            {
                animator.SetBool("IsBurning", true);
                Burn();
                return;
            }
            animator.SetBool("IsBurning", false);

            currentCombo = 1;

        }
    }

    public void toogleInAnimationFlag()
    {
        inAnimation = !inAnimation;
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

        if (!inAnimation)
        {
            float horizontalMovement = controlManager.GetHorizontalMovement();
            float verticalMovement = controlManager.GetVerticalMovement();

            if (Math.Abs(horizontalMovement) != 0f || Math.Abs(verticalMovement) != 0f)
            {
                movement += transform.forward * controlManager.GetHorizontalMovement();
                movement += transform.right * controlManager.GetVerticalMovement() * -1;
            }
            
            animator.SetBool("IsRunning", !inAnimation && (Math.Abs(movement.x) > 0.5f || Math.Abs(movement.z) > 0.5f));
        }

        movement *= character.speed;

        if (groundChecker.isGrounded)
        {
            jumps = 0;
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
        if (collision.gameObject.CompareTag(otherPlayer.tag))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                collision.gameObject.GetComponent<MovementManager>().AddImpulse(impulse * 100f);
            }
        }
    }

    public void AddImpulse(float impulse)
    {
        this.impulse += impulse;
    }

    public abstract void SoftAttack();
    public abstract void HardAttack();
    public abstract void Jump();
    public abstract void Defend();
    public abstract void Dash();
    public abstract void Burn();

}