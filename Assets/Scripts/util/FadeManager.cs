using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour {

    public static FadeManager instance;

    public float fadeTime = 1f;
    private float fadeTimeRuntime = 0.25f;

    private CanvasGroup group;

	// Use this for initialization
	void Awake () {
        Time.timeScale = 1;

        if (instance == null)
        {
            FadeManager.instance = this;
        }

        group = gameObject.GetComponentInChildren<CanvasGroup>();

        StartCoroutine(FadeIn());
	}


    public void LoadScene(int scene)
    {
        gameObject.SetActive(true);

        StartCoroutine(FadeOut(scene));
    }

    IEnumerator FadeOut(int scene)
    {
        fadeTimeRuntime = fadeTime;
        print(fadeTimeRuntime);

        while (fadeTimeRuntime > 0)
        {
            group.alpha = 1 - fadeTimeRuntime / fadeTime;
            fadeTimeRuntime -= Time.unscaledDeltaTime;
            yield return 0;
        }

        group.alpha = 1;

        SceneManager.LoadScene(scene);
    }

    IEnumerator FadeIn()
    {
        fadeTimeRuntime = fadeTime;

        while (fadeTimeRuntime > 0)
        {
            group.alpha = fadeTimeRuntime / fadeTime;
            fadeTimeRuntime -= Time.unscaledDeltaTime;
            yield return 0;
        }

        group.alpha = 0;

        gameObject.SetActive(false);
    }
}
