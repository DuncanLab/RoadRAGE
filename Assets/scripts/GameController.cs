using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public StartController startController;
    public static GameData data;

	// Use this for initialization
	void Start () {
        // Carry over data.
        startController = (StartController) GameObject.Find("StartScriptHolder").GetComponent("StartController");
        data = startController.data;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
