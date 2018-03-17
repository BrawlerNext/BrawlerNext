using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleKiller : MonoBehaviour
{

	public float aliveTime = 2;
	
	void Awake () {
		Destroy(gameObject, aliveTime);
	}
}
