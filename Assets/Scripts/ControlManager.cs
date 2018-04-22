using util;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    private Player player;
    private Controller controller;
    private PlayerManager playerManager;

    public void Init(Controller controller, Player player, PlayerManager playerManager)
    {
        this.controller = controller;
        this.player = player;
        this.playerManager = playerManager;
    }

    public bool IsDefending()
    {
        return IsPressingButton("Defend");
    }

    public bool IsJumping()
    {
        return GetButton("Jumping");
    }

    public bool IsHardAttacking()
    {
        return GetButton("HardAttack");
    }

    public bool IsSoftAttacking()
    {
        return GetButton("SoftAttack");
    }

    public bool IsDashing()
    {
       
        return GetButton("Dash");
    }

    public bool IsBurning()
    {
        return false;
    }

    public bool IsCancelTargeting()
    {
        return IsPressingButton("Target");
    }

    public float GetHorizontalMovement()
    {
        return GetAxis("Horizontal");
    }

    public float GetVerticalMovement()
    {
        return GetAxis("Vertical");
        
  }

    private float GetAxis(string axis)
    {
        if (controller != Controller.IA) // TODO
        {
            float normalized = Mathf.Abs(Input.GetAxisRaw(BuildControlName(axis))) > 0.3f ? 1 : 0;
            float sign = Mathf.Sign(Input.GetAxisRaw(BuildControlName(axis)));
         
            return normalized * sign;
        }

        return 0;
    }

    private bool GetButton(string name)
    {
        if (controller == Controller.IA) return GetIaDesition();

        return Input.GetButtonDown(BuildControlName(name));
    }

    private bool IsPressingButton(string name)
    {
        if (controller == Controller.IA) return GetIaDesition();

        return Input.GetButton(BuildControlName(name));
    }

    private string BuildControlName(string control)
    {
        return player + "" + controller + control;
    }

    private bool GetIaDesition()
    {
        if (controller == Controller.IA)
        {
            return Random.value >= 0.5f;
        }

        return false;
    }
}