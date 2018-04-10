using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{

	private PlayerManager player1;
	private PlayerManager player2;

	public static bool DebugginGame = false;
	
	public bool Debugging = false;

	private void Awake()
	{
		GameDirector.DebugginGame = Debugging;
		
		player1 = GameObject.FindGameObjectWithTag("P1").GetComponent<PlayerManager>();
		player2 = GameObject.FindGameObjectWithTag("P2").GetComponent<PlayerManager>();

		ResetPositions();
		
	}

	private void ResetPositions()
	{
		player1.ResetPosition();
		player2.ResetPosition();

		if (Debugging)
		{
			player1.transform.position = player2.transform.position + player2.transform.forward * 3f;
		}
	}

	private void Update()
	{
		if (player1.isDead())
		{
			Debug.Log("Player 1 dies.");
			ResetPositions();
			player1.GetComponent<PlayerManager>().PlaySoundOf(AudioType.DEATH);
		}
		
		if (player2.isDead())
		{
			Debug.Log("Player 2 dies.");
			ResetPositions();
			player2.GetComponent<PlayerManager>().PlaySoundOf(AudioType.DEATH);
		}
	}
}
