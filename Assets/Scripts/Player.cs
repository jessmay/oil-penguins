using UnityEngine;
using System.Collections;

public class Player : Agent {

	private const float turnStep = 5.0f;
	private const float moveStep = 10.0f;//0.15f;

	private int numFeelers = 3;
	private float[] feelers;
	private int feelerLength;

	// Use this for initialization
	void Start () {
		base.Start();
		feelerLength = (int)(GetComponent<CircleCollider2D>().radius*3);
	}
	
	// Update is called once per frame
	void Update () {

		//if turning CCW, increment heading
		if (Input.GetKey(KeyCode.LeftArrow)) {
			heading+=turnStep;
			if(heading >= 360) heading%=360;
			transform.rotation *= Quaternion.Euler (0,0, turnStep);
		}

		//If turning CW, decrement heading
		if (Input.GetKey(KeyCode.RightArrow)) {
			heading-=turnStep;
			if(heading < 0) heading = (heading+360)%360;
			transform.rotation *= Quaternion.Euler (0,0,-turnStep);
		}

		//If moving forward
		if (Input.GetKey(KeyCode.UpArrow)) {
			Vector2 temp = new Vector2(transform.up.x, transform.up.y);
			velocity = temp * (float)moveStep;
		}

		//If moving backward
		if (Input.GetKey(KeyCode.DownArrow)) {
			Vector2 temp = new Vector2(transform.up.x, transform.up.y);
			velocity = temp * (float)-moveStep;
		}

		feelers = getLengthOfFeelers(feelerLength, numFeelers);

		//displayFeelers(feelers);

		Move();
	}

}
