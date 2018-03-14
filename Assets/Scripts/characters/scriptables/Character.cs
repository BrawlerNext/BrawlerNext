using System;
using UnityEngine;

namespace characters.scriptables
{
	[CreateAssetMenu(fileName = "Character", menuName = "BrawlerNext/Character", order = 0)]
    [Serializable]
	public class Character : ScriptableObject {
		public float speed = 10;
		
		public int maxJumps = 2;
		public float jumpForce = 250;

		public float softPunchDamage = 25;
		public float hardPunchDamage = 25;

		public float shieldRepairDelay = 3;
		public float shieldLife = 100;
		public float shieldRepairVelocity = 10;
		public float shieldDamagedReducedPercentage = 0.5f;

        public AudioEntry[] clips;
        public Particle[] particles;
	}
}