using util;
using UnityEngine;

public class ControlManager : MonoBehaviour {
  private Player player;
  private Controller controller;

  public void Init(Controller controller, Player player)
  {
    this.controller = controller;
    this.player = player;
  }
  
  private void Update()
  {
    
  }
  
  public bool IsDefending()
  {
      return false;
  }

  public bool IsJumping()
  {
    return GetButton("Jumping");
  }

  public bool IsHardAttacking()
  {
      return false;
  }

  public bool IsSoftAttacking()
  {
    return GetButton("SoftAttack");
  }

  public bool IsDashing()
  {
      return false;
  }

  public bool IsBurning()
  {
      return false;
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
    return Input.GetAxisRaw(BuildControlName(axis));
  }
  
  private bool GetButton(string name)
  {
    return Input.GetButtonDown(BuildControlName(name));
  }


  private string BuildControlName(string control)
  {
    return player + "" + controller + control;
  }
}