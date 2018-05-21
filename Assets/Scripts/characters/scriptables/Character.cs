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

		public float dashSpeed = 20;
		public float dashTimeInSeconds = 1.5f;
		public AnimationCurve dashCurve;

		public float softPunchDamage = 25;
		public float hardPunchDamage = 75;
        public float criticChance = 0.3f;

        public float aeroPunchDamage = 50;
        public float aeroPunchSpeed = 10;
        public float aeroStopTime = 0.5f;
        public float aeroHitMaxTime = 2.0f;
        public float aeroMaxDistance = 10;
		public float aeroAscensionDistance = 0.1f;

		public float shieldRepairDelay = 3;
		public float shieldLife = 100;
		public float shieldDecayVelocity = 10;
		public float shieldRepairVelocity = 5;
		public float shieldDamagedReducedPercentage = 0.5f;

        public AudioEntry[] clips;
        public Particle[] particles;
    }
}