using characters.scriptables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager {

    private Character character;

    /**
     * Take care, this method could return null if the particle type is not found
     * */
	public GameObject RetrieveParticle(ParticleType type)
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

    public ParticleManager(Character character)
    {
        this.character = character;
    }
}