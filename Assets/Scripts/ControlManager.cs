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
      return GetButton("HardAttack");
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

  public float GetCameraMovement()
  {
      return GetAxis("Camera");
  }

  private float GetAxis(string axis)
  {
    float normalized = Mathf.Abs(Input.GetAxisRaw(BuildControlName(axis))) > 0.3f ? 1 : 0;
    float sign = Mathf.Sign(Input.GetAxisRaw(BuildControlName(axis)));
    return normalized * sign;
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