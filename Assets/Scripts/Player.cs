﻿using UnityEngine;
using System.Collections;

public class Player : Agent {

	private const double turnStep = 5.0;
	private const double moveStep = 10.0;//0.15;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//if turning CCW, increment heading
		if (Input.GetKey(KeyCode.LeftArrow)) {
			heading+=turnStep;
			if(heading >= 360) heading%=360;
			transform.rotation = Quaternion.Euler (0,0, (float)heading);
		}

		//If turning CW, decrement heading
		if (Input.GetKey(KeyCode.RightArrow)) {
			heading-=turnStep;
			if(heading < 0) heading = (heading+360)%360;
			transform.rotation = Quaternion.Euler (0,0,(float)heading);
		}

		//If moving forward
		if (Input.GetKey(KeyCode.UpArrow)) {
			velocity = transform.up * (float)moveStep;
		}

		//If moving backward
		if (Input.GetKey(KeyCode.DownArrow)) {
			velocity = transform.up * (float)-moveStep;
		}

		Move();
	}


}
