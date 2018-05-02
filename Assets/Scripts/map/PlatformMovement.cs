using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour {

    public Vector3 initialPosition;
    public Vector3 finalPosition;

    protected bool startToEnd = true;

    private void Awake() {
        this.transform.position = initialPosition;
    }

    private void Update()
    {
        Vector3 currentPosition = this.transform.position;

        Vector3 newPos;
        bool changeDirection;

        if (startToEnd)
        {
            newPos = Vector3.Lerp(currentPosition, finalPosition, Time.deltaTime);

            changeDirection = Mathf.Abs(Vector3.Distance(currentPosition, finalPosition)) < 0.5f;
        }
        else
        {
            newPos = Vector3.Lerp(currentPosition, initialPosition, Time.deltaTime);

            changeDirection = Mathf.Abs(Vector3.Distance(currentPosition, initialPosition)) < 0.5f;
        }

        this.transform.position = newPos;

        if (changeDirection)
            {
                startToEnd = !startToEnd;
            }

        
    }
}
