using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotto : MonoBehaviour
{
    public GameObject shotOne;
    public GameObject shotTwo;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {

		if(Input.GetKeyDown(KeyCode.F))
        {
             GameObject CloneShot = Instantiate(shotOne, transform.position, Quaternion.identity);
             Destroy(CloneShot, 2f);
        }
	}
}
