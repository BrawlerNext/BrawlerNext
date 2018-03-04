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
    throw new System.NotImplementedException();
  }

  public bool IsJumping()
  {
    return GetButton("Jumping");
  }

  public bool IsHardAttacking()
  {
    throw new System.NotImplementedException();
  }

  public bool IsSoftAttacking()
  {
    throw new System.NotImplementedException();
  }

  public bool IsDashing()
  {
    throw new System.NotImplementedException();
  }

  public bool IsBurning()
  {
    throw new System.NotImplementedException();
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
    return Input.GetButton(BuildControlName(name));
  }


  private string BuildControlName(string control)
  {
    return player + "" + controller + control;
  }
}