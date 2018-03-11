using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{

	private Transform player1;
	private Transform player2;

	private void Awake()
	{
		player1 = GameObject.FindGameObjectWithTag("P1").transform;
		player2 = GameObject.FindGameObjectWithTag("P2").transform;

		ResetPositions();
	}

	private void ResetPositions()
	{
		player1.position = GameObject.FindGameObjectWithTag("RespawnP1").transform.position;
		player2.position = GameObject.FindGameObjectWithTag("RespawnP2").transform.position;
	}

	private void Update()
	{
		if (player1.position.y < -10)
		{
			Debug.Log("Player 1 dies.");
			ResetPositions();
		}
		
		if (player2.position.y < -10)
		{
			Debug.Log("Player 2 dies.");
			ResetPositions();
		}
	}
}
