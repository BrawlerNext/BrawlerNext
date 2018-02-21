using UnityEngine;

public abstract class ControlManager : MonoBehaviour {
  public Player player;

  public abstract bool IsDefending();
  public abstract bool IsJumping();
  public abstract bool IsHardAttacking();
  public abstract bool IsSoftAttacking();
  public abstract bool IsDashing();
  public abstract bool IsBurning();
}