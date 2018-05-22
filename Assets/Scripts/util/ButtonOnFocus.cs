using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonOnFocus : MonoBehaviour, ISelectHandler, IDeselectHandler {

	public Image marker;

	private void Start() {
		Component[] images = GetComponentsInChildren(typeof(Image), true);

		foreach (Image image in images) {
			if (image.gameObject.CompareTag("ButtonMarker")) {
				marker = image;
			}
		}

		marker.enabled = false;
	}

	public void OnSelect(BaseEventData data)
     {
		 marker.enabled = true;
     }

    public void OnDeselect(BaseEventData data)
     {
		 marker.enabled = false;
     }
}
