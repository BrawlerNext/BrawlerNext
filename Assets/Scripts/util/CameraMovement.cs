using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float offsetHorizontal = 10;

    public float offsetVertical;
    public float verticalRotationOffset;
    
    private Transform player1;
    private Transform player2;

	void Awake () {
        player1 = GameObject.FindGameObjectWithTag("P1").transform;
        player2 = GameObject.FindGameObjectWithTag("P2").transform;
	}
	
	void Update () {
        Vector3 midpoint = (player1.position + player2.position) * 0.5f;

        float distance = Vector3.Distance(player1.position, player2.position);

        midpoint += new Vector3(0, offsetVertical, 0);

        Quaternion rotation = Quaternion.LookRotation((midpoint - transform.position).normalized);
        
        this.transform.rotation = rotation;
        
        midpoint += new Vector3(distance, verticalRotationOffset, 0);

        midpoint.x = Mathf.Max(midpoint.x, offsetHorizontal);

        transform.position = Vector3.Lerp(transform.position, midpoint, Time.deltaTime);
	}
}
