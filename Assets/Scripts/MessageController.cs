using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour {

	private Vector3 startPosition;
	private Vector3 endPosition;
	private bool messageReceived = false;

	private Text gui;

	public void ShowMessage(string message) {
		this.transform.localPosition = startPosition;
		messageReceived = true;
		this.gui.text = message;
	}

	private void Awake() {
		startPosition = this.transform.localPosition;
		endPosition = startPosition + new Vector3(0, 100, 0);
		gui = gameObject.GetComponent<Text>();
	}

	private void Update() {
		Vector3 currentPosition = this.transform.localPosition;

    if (messageReceived)
    {
      messageReceived = !(Mathf.Abs(Vector3.Distance(currentPosition, endPosition)) < 10f);

      this.transform.localPosition = Vector3.Lerp(currentPosition, endPosition, Time.deltaTime);
    } else {
			this.gui.text = "";
		}
	}

}
