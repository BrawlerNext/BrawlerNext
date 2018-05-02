using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstConfiguration : MonoBehaviour {

	public FirstEventObjects type;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FirstEvent() {
		switch (type)
		{
				case FirstEventObjects.PLATFORM_1:
					
					break;
		}
	}
}

public enum FirstEventObjects {
	PLATFORM_1
}
