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
            if (controlManager.IsJumping())
            {
                if (groundChecker.isGrounded || jumps < character.maxJumps ||
                    (jumps == character.maxJumps && controlManager.IsBurning()))
                {
                    Jump();
                    jumps++;
                }

                return;
            }

            if (controlManager.IsSoftAttacking())
            {
                animator.SetBool("IsSoftAttacking", true);
                SoftAttack();
                return;
            }
            else
            {
                animator.SetBool("IsSoftAttacking", false);
            }

            if (controlManager.IsHardAttacking())
            {
                return;
            }

            if (controlManager.IsDefending())
            {
                return;
            }

            if (controlManager.IsDashing())
            {
                return;
            }

            if (controlManager.IsBurning())
            {
                return;
            }
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

        rb.velocity = movement;
    }

    public abstract void SoftAttack();
    public abstract void HardAttack();
    public abstract void Jump();

}