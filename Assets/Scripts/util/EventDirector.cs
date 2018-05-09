using System.Collections;
using UnityEngine;

public class EventDirector : MonoBehaviour
{

    public GameDirector gameDirector;

    private string lastEvent = "";
    private bool eventFinished = true;

    private void Start()
    {
        gameDirector = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameDirector>();
    }

    public string InitMapConfig()
    {

        string eventName = "";
        if (eventFinished)
        {
            if (lastEvent == "SecondEvent" || lastEvent == "" || true)
            {
                eventName = "FirstEvent";
            }
            else
            {
                eventName = "SecondEvent";
            }

            lastEvent = eventName;

            if (eventName == "FirstEvent")
            {
                gameDirector.ShowMessage("El ascenso se acerca...");
            }
            else
            {
                gameDirector.ShowMessage("¡El centro se agrandará!");
            }

            eventFinished = false;
            StartCoroutine(StartEvent(eventName));
            StartCoroutine(EndEvent(eventName));
        }

        return eventName;
    }

    IEnumerator StartEvent(string eventName)
    {
        yield return new WaitForSeconds(5);
        if (eventName == "FirstEvent")
        {
            gameDirector.ShowMessage("Ascendiendo...");
        }
        else
        {
            gameDirector.ShowMessage("¡El centro se agranda!");
        }
        gameObject.BroadcastMessage(eventName, true);
    }

    IEnumerator EndEvent(string eventName)
    {
        yield return new WaitForSeconds(25);
        if (eventName == "FirstEvent")
        {
            gameDirector.ShowMessage("El ascenso decaerá...");
        }
        else
        {
            gameDirector.ShowMessage("¡El gran centro desaparecerá!");
        }
        yield return new WaitForSeconds(7.5f);
        gameObject.BroadcastMessage(eventName, false);
        eventFinished = true;
    }
}
