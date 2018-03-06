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

        controlManager = gameObject.GetComponent<ControlManager>();
        controlManager.Init(controller, player);
    }

    protected void Update()
    {
        Move();

        if (!inAnimation)
        {
            Debug.Log(!groundChecker.isGrounded);
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

    public void startAnimation()
    {
        inAnimation = true;
    }

    public void stopAnimation()
    {
        inAnimation = false;
    }


    protected void Move()
    {
        Vector3 movement = Vector3.zero;

        if (!inAnimation)
        {
            movement = new Vector3(controlManager.GetHorizontalMovement(), 0.0f,
                controlManager.GetVerticalMovement());
        }

        movement *= character.speed;

        if (!groundChecker.isGrounded)
        {
            movement.y = rb.velocity.y + Physics.gravity.y * character.gravityScale * Time.deltaTime;
        }
        else
        {
            jumps = 0;
        }

        animator.SetBool("IsRunning", !inAnimation && (Math.Abs(movement.x) > 5f || Math.Abs(movement.z) > 5f));

        if (movement.x > 0.1f && movement.z > 0.1f)
        {
            rb.velocity = movement;
        }
    }

    public abstract void SoftAttack();
    public abstract void HardAttack();
    public abstract void Jump();
    public abstract void Defend();
    public abstract void Dash();
    public abstract void Burn();

}