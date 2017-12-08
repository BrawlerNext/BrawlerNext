using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		// Movement

		// Skills
		switch (getKeyKode()) {
			case KeyCode.Mouse0:
				SoftAttackSkill();
			break;
			case KeyCode.Mouse1:
				HardAttackSkill();
			break;
			case KeyCode.Space:
				JumpSkill();
			break;
		}
	}

	public abstract void SoftAttackSkill();
	public abstract void HardAttackSkill();
	public abstract void JumpSkill();

    private KeyCode getKeyKode()
    {
		foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
		{
			if (keyCode.ToString() == Input.inputString)
				return keyCode;
		}

		return KeyCode.Mouse0;
    }
}
