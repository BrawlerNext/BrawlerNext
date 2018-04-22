using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{

    private PlayerManager player1;
    private int player1Deaths = 0;
    private PlayerManager player2;
    private int player2Deaths = 0;

    public static bool DebugginGame = false;

    public bool Debugging = false;

    private void Start()
    {
        GameDirector.DebugginGame = Debugging;

        player1 = GameObject.Find("P1").GetComponent<PlayerManager>();
        player2 = GameObject.Find("P2").GetComponent<PlayerManager>();

        InitializePosition();

    }

    public int GetDeathsOf(Player player)
    {
        return player == player1.player ? player1Deaths : player2Deaths;
    }

    private void InitializePosition()
    {
        player1.Reset();
        player2.Reset();
    }
    /*if (Debugging)
    {
        player1.transform.position = player2.transform.position + player2.transform.forward * 3f;
    }*/
    //}

    private void Update()
    {
        if (player1.isDead())
        {
            Debug.Log("Player 1 dies.");
            player1Deaths++;
            player1.Reset();
            player1.GetComponent<PlayerManager>().PlaySoundOf(AudioType.DEATH);
        }

        if (player2.isDead())
        {
            Debug.Log("Player 2 dies.");
            player2Deaths++;
            player2.Reset();
            player2.GetComponent<PlayerManager>().PlaySoundOf(AudioType.DEATH);
        }

        if (player1Deaths >= 2 || player2Deaths >= 2)
        {

            if (player1Deaths >= 2)
                PlayerPrefs.SetInt("winner", 2);
            else
                PlayerPrefs.SetInt("winner", 1);
            SceneManager.LoadScene("Winner");
        }

    }



}
