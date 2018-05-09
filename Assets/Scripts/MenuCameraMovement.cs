﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCameraMovement : MonoBehaviour {
    public GameObject characterSelection;
    public GameObject mapSelection;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
	}

    public void FadePositionToCharacterSelection()
    {
        this.transform.position = characterSelection.transform.position;
    }

    public void FadePositionToMapSelection()
    {
        this.transform.position = mapSelection.transform.position;
    }

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
