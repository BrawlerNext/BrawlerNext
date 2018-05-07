using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{

  private PlayerManager player1;
  public int player1Deaths = 0;
  private PlayerManager player2;
  public int player2Deaths = 0;

  public EventDirector eventDirector;
  public MessageController messageController;

  public static bool DebugginGame = false;

  internal void removeDeathFrom(Player player)
  {
    switch (player)
    {
      case Player.P1:
        player1Deaths--;
        player1Deaths = Math.Max(0, player1Deaths);
        break;
      case Player.P2:
        player2Deaths--;
        player2Deaths = Math.Max(0, player2Deaths);
        break;
    }
  }

  internal void ShowMessage(string v)
  {
    messageController.ShowMessage(v);
  }

  public bool Debugging = false;

  private void Start()
  {
    GameDirector.DebugginGame = Debugging;

    player1 = GameObject.Find("P1").GetComponent<PlayerManager>();
    player2 = GameObject.Find("P2").GetComponent<PlayerManager>();

    messageController = GameObject.FindGameObjectWithTag("MessageController").GetComponent<MessageController>();

    InitializePosition();

    if (!Debugging) {
      FreezePlayers(true);

      StartCoroutine(StartCounter());
    }

    InitializeEvents();
  }

  private IEnumerator StartCounter()
  {
      messageController.ShowMessage("La partida comienza en...");
    yield return new WaitForSeconds(1);
    messageController.ShowMessage("3");
    yield return new WaitForSeconds(1);
    messageController.ShowMessage("2");
    yield return new WaitForSeconds(1);
    messageController.ShowMessage("1...");
    yield return new WaitForSeconds(1);
    messageController.ShowMessage("¡A luchar!");
    FreezePlayers(false);
  }

  private void FreezePlayers(bool freeze)
  {
    player1.Freeze(freeze);
    player2.Freeze(freeze);
  }

  void InitializeEvents()
  {
    if (player1Deaths != 0 || player2Deaths != 0)
    {
      eventDirector.InitMapConfig();
    }
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

  private void RoundMessage() {
      if (player1Deaths + player2Deaths == 2) {
          messageController.ShowMessage("¡Última ronda!");
      } else {
          messageController.ShowMessage("Ronda " + (player1Deaths + player2Deaths + 1));
      }
  }

  private void Update()
  {
    if (player1.isDead())
    {
      player1Deaths++;
      InitializePosition();
      player1.GetComponent<PlayerManager>().PlaySoundOf(AudioType.DEATH);
      InitializeEvents();
    }

    if (player2.isDead())
    {
      player2Deaths++;
      InitializePosition();
      player2.GetComponent<PlayerManager>().PlaySoundOf(AudioType.DEATH);
      InitializeEvents();
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