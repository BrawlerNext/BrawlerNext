using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementManager : MonoBehaviour
{
    private int jumps = 0;

    public Character character;

    protected Rigidbody rb;
    protected GroundChecker groundChecker;
    protected ControlManager controlManager;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        groundChecker = gameObject.GetComponentInChildren<GroundChecker>();
    }

    protected void FixedUpdate()
    {
        // Movement
        Move();

        if (controlManager.IsJumping()) {
            if (groundChecker.isGrounded || jumps < character.maxJumps || (jumps == character.maxJumps && controlManager.IsBurning())){
                Jump();
                jumps++;
            }

            return;
        }

        if (controlManager.IsDefending()) {
            return;
        }

        if (controlManager.IsSoftAttacking()) {
            SoftAttack();
            return;
        }

        if (controlManager.IsHardAttacking()) {
            return;
        }

        if (controlManager.IsDashing()) {
            return;
        }

        if (controlManager.IsBurning()) {
            return;
        }
    }


    protected void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        
		movement *= character.speed;
		
		if (!groundChecker.isGrounded){
			movement.y = rb.velocity.y + Physics.gravity.y * character.gravityScale * Time.deltaTime;
		} else {
			jumps = 0;
		}

        rb.velocity = movement;
    }

    public abstract void SoftAttack();
    public abstract void HardAttack();
    public abstract void Jump();

    private KeyCode getCurrentKeyCode()
    {
        if(Input.GetKeyDown(KeyCode.Space))
		{
			return KeyCode.Space;
		}

        if(Input.GetKeyDown(KeyCode.Mouse0))
		{
			return KeyCode.Mouse0;
		}

        if(Input.GetKeyDown(KeyCode.Mouse1))
		{
			return KeyCode.Mouse1;
		}

		return KeyCode.Return;
    }
}
