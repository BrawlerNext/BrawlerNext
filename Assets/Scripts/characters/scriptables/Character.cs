using UnityEngine;

namespace characters.scriptables
{
	[CreateAssetMenu(fileName = "Character", menuName = "BrawlerNext/Character", order = 0)]

	public class Character : ScriptableObject {
		public float speed = 10;
        public float softPunchDamage = 25;
        public float hardPunchDamage = 25;
        public float kickDamage = 25;
		public float jumpForce = 250;
		public int gravityScale = 1;
		public int maxJumps = 2;
	}
}