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
    protected Collider kickCollider;

    protected Rigidbody rb;
    protected GroundChecker groundChecker;
    protected ControlManager controlManager;
    protected Animator animator;

    protected bool inAnimation = false;
    protected int jumps = 0;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        groundChecker = gameObject.GetComponentInChildren<GroundChecker>();
        animator = gameObject.GetComponentInChildren<Animator>();

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
                case "KickCollider":
                    kickCollider = allColiders[i];
                    kickCollider.enabled = false;
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

            if (controlManager.IsSoftAttacking())
            {
                animator.SetBool("IsSoftAttacking", true);
                SoftAttack();
                return;
            }
            animator.SetBool("IsSoftAttacking", false);

            if (controlManager.IsHardAttacking())
            {
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
            movement = new Vector3(controlManager.GetHorizontalMovement(), 0.0f,
                controlManager.GetVerticalMovement());

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

            if (contact.thisCollider.tag.Contains("KickCollider"))
            {
                impulse = character.kickDamage;
            }
        }
        if (collision.gameObject.CompareTag("Hiteable"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * impulse * 100, ForceMode.Impulse);
                //rb.AddForce(transform.forward * impulse * 100 * -1, ForceMode.Impulse);
            }
        }
    }

    public abstract void SoftAttack();
    public abstract void HardAttack();
    public abstract void Jump();
    public abstract void Defend();
    public abstract void Dash();
    public abstract void Burn();

}