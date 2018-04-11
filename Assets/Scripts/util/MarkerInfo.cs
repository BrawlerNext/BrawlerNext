using UnityEngine;
using UnityEngine.UI;
using util;

public class MarkerInfo : MonoBehaviour {
  public Player player;
  private GameDirector gameDirector;
  private Text textUI;

  private void Start() {
    textUI = GetComponent<Text>();
    gameDirector = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
  }
  
  private void Update() {
    textUI.text = gameDirector.GetDeathsOf(player) +  "";
  }
}