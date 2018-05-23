using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinnerScript : MonoBehaviour
{
    public GameObject winner1;
    public GameObject winner2;
    public GameObject panelMenu;
    private int winner;
    // Use this for initialization


    void Start()
    {

        winner = PlayerPrefs.GetInt("winner");
        if (winner == 1)
        {
            winner1.SetActive(true);
        }
        else
        {
            winner2.SetActive(true);
        }
        StartCoroutine(Waitforpause());

    }

    // Update is called once per frame
    public void PlayAgainMenu(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    IEnumerator Waitforpause()
    {
        yield return new WaitForSecondsRealtime(2f);
        panelMenu.SetActive(true);
        Debug.Log("funcionaloco");
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
