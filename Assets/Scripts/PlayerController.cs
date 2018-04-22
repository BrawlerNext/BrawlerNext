using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public enum states
    {
        IDLE,
		MOVING,
        JUMPING,
        DASHING,
        DEFENDING,
        LIGHTATTACKING,
        HEAVYATTACKING,
        AERIALHITTING,
        STUNNED,
        PUSHED,
        DEAD,
        RESPAWNNING
    }

    private states state;
	public Animator animator;

    void Awake()
    {
		animator.SetBool("IsMoving", false);
		animator.SetBool("IsLightAttaking", false);
		animator.SetBool("IsLightAttacking", false);
		animator.SetBool("IsHeavyAttacking", false);
		animator.SetBool("IsAerialAttacking", false);
		animator.SetBool("IsJumping", false);
		animator.SetBool("IsFalling", false);
		animator.SetBool("IsDashing", false);
		animator.SetBool("IsDefending", false);
		animator.SetBool("IsPushed", false);
		animator.SetBool("IsStunned", false);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case states.IDLE:
			break;

			case states.MOVING:
			break;

			case states.JUMPING:
			break;
			
			case states.DASHING:
			break;

			case states.DEFENDING:
			break;

			case states.LIGHTATTACKING:
			break;

			case states.HEAVYATTACKING:
			break;

			case states.AERIALHITTING:
			break;

			case states.STUNNED:
			break;

			case states.PUSHED:
			break;

			case states.DEAD:
			break;

			case states.RESPAWNNING:
			break;
        }
    }





}



