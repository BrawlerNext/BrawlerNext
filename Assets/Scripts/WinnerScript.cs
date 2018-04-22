using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerScript : MonoBehaviour
{
    public GameObject winner1;
    public GameObject winner2;
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
