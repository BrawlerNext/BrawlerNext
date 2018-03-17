using characters.scriptables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ParticleManager {

    private readonly Character character;

	public ParticleManager(Character character)
	{
		this.character = character;
	}

	private GameObject RetrieveParticle(ParticleType type)
	{
		foreach (Particle particle in character.particles)
		{
			if (particle.type == type)
			{
                return particle.particle;
			}
		}

        return null;
	}

	public void InstantiateParticle(ParticleType particleType, Vector3 positionToInstantiate)
	{
		GameObject particle = RetrieveParticle(particleType);
        
		if (particle != null)
		{
			Object.Instantiate(particle, positionToInstantiate, Quaternion.identity);
		}
		else
		{
			if (particleType != ParticleType.NONE)
			{
				Debug.LogWarning("Particle type: " + particleType.ToString() + " not found in " + character.ToString() + " particles!");
			}
		}
	}
}