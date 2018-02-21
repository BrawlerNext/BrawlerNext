using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "BrawlerNext/Character", order = 0)]
public class Character : ScriptableObject {
  public float speed = 10;
	public float jumpForce = 1000;
	public int gravityScale = 1;
	public int maxJumps = 2;
}