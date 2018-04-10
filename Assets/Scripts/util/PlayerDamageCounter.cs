using UnityEngine;
using UnityEngine.UI;
using util;

public class PlayerDamageCounter : MonoBehaviour {
  public Player player;
  private PlayerManager playerManager;
  private Text damageText;

  private void Start() {
    playerManager = GameObject.FindGameObjectWithTag(player.ToString()).GetComponent<PlayerManager>();
    damageText = GetComponent<Text>();
  }

  private void Update() {
    damageText.text = playerManager.GetDamage() + " %";
  }
}