using UnityEngine;

namespace util
{
  [System.Serializable]
  public class PunchCollider {
    public ColliderType colliderType;
    public Collider collider;
  }

  public enum ColliderType {
    HARD_PUNCH, SOFT_PUNCH
  }
}