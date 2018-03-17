using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Particle
{
    public ParticleType type;
    public GameObject particle;
}

public enum ParticleType
{
    SOFT_HIT,
    HARD_HIT,
    DAMAGE,
    JUMP,
    DEFEND,
    DASH,
    BURN,
    DEATH,
    RESPAWN,
    NONE
}
