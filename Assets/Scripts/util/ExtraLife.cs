using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using util;

public class ExtraLife : MonoBehaviour
{

  private GameDirector gameDirector;
  void Start()
  {
    gameDirector = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
  }

  private void OnTriggerEnter(Collider other)
  {
    PlayerManager playerManager = other.GetComponent<PlayerManager>();

    if (playerManager != null)
    {
      Player player = other.GetComponent<PlayerManager>().player;
      gameDirector.removeDeathFrom(player);
			gameDirector.ShowMessage(player + " ha obtenido el favor");
			Destroy(gameObject);
    }
  }
}
