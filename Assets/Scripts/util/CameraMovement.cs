using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public float offsetHorizontal = 10;

    public float offsetVertical;
    public float verticalRotationOffset;
    public float zoomFactor;
    public float minDistanceFromMidPoint;

    private Transform player1;
    private Transform player2;

    public float player1Weight;
    public float player2Weight;

    public bool weights;

    void Awake()
    {
        player1 = GameObject.FindGameObjectWithTag("P1").transform;
        player2 = GameObject.FindGameObjectWithTag("P2").transform;
    }

    void Update()
    {

        player1Weight = Vector3.Distance(player1.position, this.transform.position);
        player2Weight = Vector3.Distance(player2.position, this.transform.position);
        float sum = player1Weight + player2Weight;
        player1Weight = 1f - player1Weight / sum;
        player2Weight = 1f - player2Weight / sum;


        Vector3 midpoint = (player1.position + player2.position) * 0.5f;

		midpoint = midpoint + (player1.position - midpoint) * player1Weight + (player2.position - midpoint) * player2Weight;

        Debug.DrawLine(transform.position, midpoint, Color.yellow);
        Debug.DrawLine(player1.position, midpoint, Color.red);
        Debug.DrawLine(player2.position, midpoint, Color.green);

        float distance = Vector3.Distance(player1.position, player2.position) * zoomFactor;

        midpoint += new Vector3(0, offsetVertical, 0);

        Quaternion rotation = Quaternion.LookRotation((midpoint - transform.position).normalized);

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, Time.deltaTime);

        midpoint += new Vector3(distance + minDistanceFromMidPoint, verticalRotationOffset, 0);

        transform.position = Vector3.Lerp(transform.position, midpoint, Time.deltaTime);
    }
}
