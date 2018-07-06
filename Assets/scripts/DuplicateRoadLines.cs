using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateRoadLines : MonoBehaviour {

    public GameObject RoadLine;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 20; i++)
        {
            var newPos = new Vector3(0, 0, i * 10);
            Instantiate(RoadLine, newPos, Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
