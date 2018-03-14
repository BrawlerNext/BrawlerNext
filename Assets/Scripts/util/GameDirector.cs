using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{

	private Transform player1;
	private Transform player2;

	public static bool DebugginGame = false;
	
	public bool Debugging = false;

	private void Awake()
	{
		GameDirector.DebugginGame = Debugging;
		
		player1 = GameObject.FindGameObjectWithTag("P1").transform;
		player2 = GameObject.FindGameObjectWithTag("P2").transform;

		ResetPositions();
		
	}

	private void ResetPositions()
	{
		player1.position = GameObject.FindGameObjectWithTag("RespawnP1").transform.position;
		player2.position = GameObject.FindGameObjectWithTag("RespawnP2").transform.position;

		if (Debugging)
		{
			player1.position = player2.position + player2.forward * 3f;
		}
	}

	private void Update()
	{
		if (player1.position.y < -10)
		{
			Debug.Log("Player 1 dies.");
			ResetPositions();
			player1.GetComponent<PlayerManager>().PlaySoundOf(AudioType.DEATH);
		}
		
		if (player2.position.y < -10)
		{
			Debug.Log("Player 2 dies.");
			ResetPositions();
            player2.GetComponent<PlayerManager>().PlaySoundOf(AudioType.DEATH);
		}
	}
}
