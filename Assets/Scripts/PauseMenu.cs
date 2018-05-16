using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;
    public GameObject controlsMenu;
    bool isGamePaused = false;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            if (isGamePaused == false)
            {
                PauseTime();

            }
            else
            {
                ResumeTime();

            }

    }


    void PauseTime()
    {
        Time.timeScale = 0;
        isGamePaused = true;
        pauseMenu.SetActive(true);
    }

    void ResumeTime()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        pauseMenu.SetActive(false);
    }


    public void ResumeGameplay()
    {
        ResumeTime();
    }

    public void RestartGameplay()
    {

    }

    public void Controls()
    {
        controlsMenu.SetActive(true);
    }

    public void BackToMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void BacktoPause()
    {
        controlsMenu.SetActive(false);

    }
}
