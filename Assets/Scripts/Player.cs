/*
Jessica May
Joshua Linge
Player.cs
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Agent {

	// Use this for initialization
	new void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {

		checkButtons();

		//Calculate information for each sensor.
		sense();

		//think

		//act
		//Move player based off velocity and heading
		Move();

	}

	private void sense() {
		
		//Get length of feelers
		feelers.calculate();
		
		//Get list of nearest agents
		adjAgents.calculate();
		
		//Get agents in pie slice angles
		pieSlices.calculate();
	}

	private void checkButtons () {
		
		//display feelers
		if (Input.GetKeyDown(KeyCode.Z)) {
			dispFeelers = !dispFeelers;
		}
		
		//display pie slices
		if(Input.GetKeyDown(KeyCode.X)){
			dispSlices = !dispSlices;
		}
		
		//display adjacent agents
		if(Input.GetKeyDown(KeyCode.C)){
			dispAdjAgent = !dispAdjAgent;
		}
		
		//Display debug text
		if(Input.GetKeyDown(KeyCode.Q)) {
			debug = !debug;
		}
		
		//if turning CCW, increment heading
		if (Input.GetKey(KeyCode.LeftArrow)) {
			lookLeft();
		}
		
		//If turning CW, decrement heading
		if (Input.GetKey(KeyCode.RightArrow)) {
			lookRight();
		}
		
		//If moving forward
		if (Input.GetKey(KeyCode.UpArrow)) {
			moveForward();
		}
		
		//If moving backward
		if (Input.GetKey(KeyCode.DownArrow)) {
			moveBackward();
		}
	}

	void OnGUI(){
		DrawDebugInformation ();
	}
}