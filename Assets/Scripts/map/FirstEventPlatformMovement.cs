using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstEventPlatformMovement : MonoBehaviour
{
  public Vector3 finalPosition;
  private Vector3 startPosition;

  private bool eventStart = false;
  private bool eventEnd = false;

	private void Start() {
		startPosition = this.transform.localPosition;
	}

  void FirstEvent(bool isStarting)
  {
    if (isStarting) {
			eventStart = true;
		} else {
			eventEnd = true;
		}
  }

  private void Update()
  {
    Vector3 currentPosition = this.transform.localPosition;

    if (eventStart)
    {
      eventStart = !(Mathf.Abs(Vector3.Distance(currentPosition, finalPosition)) < 0.1f);

      this.transform.localPosition = Vector3.Lerp(currentPosition, finalPosition, Time.deltaTime);
    }

    if (eventEnd)
    {
      eventEnd = !(Mathf.Abs(Vector3.Distance(currentPosition, startPosition)) < 0.1f);

      this.transform.localPosition = Vector3.Lerp(currentPosition, startPosition, Time.deltaTime);
    }
  }
}
