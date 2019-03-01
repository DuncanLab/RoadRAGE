using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotator : MonoBehaviour {

    private const int ROTATE_SPEED_MODIFIER = 150;

	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.position, Vector3.up, ROTATE_SPEED_MODIFIER * Time.deltaTime);
    }
}
