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

    player1 = GameObject.FindGameObjectWithTag("P1").GetComponent<PlayerManager>();
    player2 = GameObject.FindGameObjectWithTag("P2").GetComponent<PlayerManager>();

    messageController = GameObject.FindGameObjectWithTag("MessageController").GetComponent<MessageController>();

    if (!Debugging) {
      InitializePosition();

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
    player1.BasicReset();
    player2.BasicReset();
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

    if (Input.GetJoystickNames().Length > 0) {
      player1.controller = Controller.Xbox;
      player1.controlManager.Init(player1.controller, player1.player);
    }

    if (Input.GetJoystickNames().Length > 1) {
      player2.controller = Controller.Xbox;
      player2.controlManager.Init(player2.controller, player2.player);
    }

    if (player1.isDead())
    {
      player1Deaths++;
      player1.BasicReset();
      PlayerManager playerManager = player1.GetComponent<PlayerManager>();
      playerManager.PlaySoundOf(AudioType.DEATH);
      playerManager.damage = 0;
      InitializeEvents();
    }

    if (player2.isDead())
    {
      player2Deaths++;
      player2.BasicReset();
      PlayerManager playerManager = player2.GetComponent<PlayerManager>();
      playerManager.PlaySoundOf(AudioType.DEATH);
      playerManager.damage = 0;
      InitializeEvents();
    }

    if (player1Deaths >= 2 || player2Deaths >= 2)
    {
      if (player1Deaths >= 2)
        PlayerPrefs.SetInt("winner", 2);
      else
        PlayerPrefs.SetInt("winner", 1);

      FadeManager.LoadSceneTo(2);
    }

  }



}