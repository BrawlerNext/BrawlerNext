using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{

    public float speed;
	public float jumpForce;
	public int gravityScale;
	public int maxJumps = 2;
	private int jumps = 0;

    protected Rigidbody rb;
    protected GroundChecker groundChecker;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        groundChecker = gameObject.GetComponentInChildren<GroundChecker>();
    }

    protected void FixedUpdate()
    {
        // Movement
        Move();
        // Skills
        switch (getCurrentKeyCode())
        {
            case KeyCode.Mouse0:
                SoftAttackSkill();
                break;

            case KeyCode.Mouse1:
                HardAttackSkill();
                break;

            case KeyCode.Space:
				if (groundChecker.isGrounded || jumps < maxJumps){
                	JumpSkill();
					jumps++;
				}
                break;
        }

		print(groundChecker.isGrounded);
    }


    protected void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
		movement *= speed;
		
		if (!groundChecker.isGrounded){
			movement.y = rb.velocity.y + Physics.gravity.y * gravityScale * Time.deltaTime;
		} else {
			jumps = 0;
		}

        rb.velocity = movement;
    }

    public abstract void SoftAttackSkill();
    public abstract void HardAttackSkill();
    public abstract void JumpSkill();

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
