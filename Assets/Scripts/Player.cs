using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Agent {

	private const float turnStep = 5.0f;
	private const float moveStep = 10.0f;

	private int viewAngle = 180;

	private bool dispFeelers = false;
	private int numFeelers = 3;
	private Vector2[] feelers;
	private int feelerLength;

	private bool dispAdjAgent = false;
	private int adjRadius;
	public Texture circle;
	private Texture adjCircle;

	private bool dispSlices = false;
	private int numSlices = 4;
	private int pieAngle = 360;

	private float lineWidth = .5f;

	public Texture lineTexture;

	private List<Agent> near;

	private bool debug = true;

	// Use this for initialization
	new void Start () {
		base.Start();
		feelerLength = (int)(radius*3);
		adjRadius = (int)(radius*3);
	}
	
	// Update is called once per frame
	void Update () {

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

		//Move player based off velocity and heading
		Move();

		//Get length of feelers
		feelers = getLengthOfFeelers(feelerLength, numFeelers, viewAngle);

		//Get list of nearest agents
		near = background.grid.getNear (this, adjRadius);
	}

	void OnGUI(){

		string debugText = "";
		string agentSensors = "";

		//Center of player object in local world space
		Vector3 center = new Vector3(renderer.bounds.center.x, renderer.bounds.center.y);
		center.Scale(new Vector3(1, -1, 1));

		//Player's center in camera space
		Vector2 pivot = (Vector2)Camera.main.WorldToScreenPoint(center);

		//Radius length in camera space
		float radiusV = (Camera.main.WorldToScreenPoint(new Vector2(radius, 0)) - Camera.main.WorldToScreenPoint(Vector2.zero)).x;
		
		//line width length in camera space
		Vector2 width = new Vector2(lineWidth, 0);
		width = (Camera.main.WorldToScreenPoint(width) - Camera.main.WorldToScreenPoint(Vector2.zero));

		//Size of the adjacent agent sensor in camera space
		Vector2 length = new Vector2(adjRadius, 0);
		length = (Camera.main.WorldToScreenPoint(length) - Camera.main.WorldToScreenPoint(Vector2.zero));
		
		//Draw feelers
		float spaceBetween = viewAngle/(feelers.Length+1);
		for (int currentFeeler = 0; currentFeeler < feelers.Length; ++currentFeeler) {

			int angle = (int) (heading - viewAngle/2 + spaceBetween*(currentFeeler+1));
			angle = (angle + 360) % 360;
			
			Vector2 feelerVec = new Vector2(feelers[currentFeeler].magnitude, 0);
			feelerVec = (Camera.main.WorldToScreenPoint(feelerVec) - Camera.main.WorldToScreenPoint(Vector2.zero));

			if (dispFeelers)
				drawBox (pivot.x-width.x/2, pivot.y+radiusV, width.x, feelerVec.x, -angle+180, pivot);

			debugText += "Feeler["+currentFeeler+"]: "+ feelers[currentFeeler].magnitude +"\n";
		}
		debugText += "\n";


		//Draw circle for nearest agents
		if (dispAdjAgent) {

			float circleSize = 2*(width.x + length.x + radiusV);
			GUI.DrawTexture(new Rect(pivot.x-circleSize/2, pivot.y-circleSize/2, circleSize, circleSize), circle, ScaleMode.ScaleToFit);
		}
		
		int[] angles = new int[numSlices];
		//Draw pie slices
			
		spaceBetween = pieAngle/numSlices;

		for(int currSlice = 0; currSlice < numSlices; currSlice++){
			int angle = (int) ((heading-spaceBetween/2)+((currSlice-1)*spaceBetween));
			angles[currSlice] = (int)((spaceBetween/2)+(currSlice*spaceBetween));
			angle = (angle+360)%360;

			if(dispSlices) 
				drawBox (pivot.x-width.x/2, pivot.y+radiusV, width.x, length.x, -angle+180, pivot);
		}

		int[] numAgentsInSlices = new int[numSlices];
		
		//Draw lable for each agent within a pie slice or circle
		for(int i = 0; i < near.Count; i++){

			string lableText = null;
			
			if (dispAdjAgent) 
				lableText = "Within";


			Vector3 currAgent = new Vector3(near[i].renderer.bounds.center.x, near[i].renderer.bounds.center.y);
			currAgent.Scale(new Vector3(1, -1, 1));
			Vector2 pivotAgent = (Vector2)Camera.main.WorldToScreenPoint(currAgent);
			
			Vector2 playerToAgent = near[i].renderer.bounds.center-renderer.bounds.center;
			Vector2 headingVec = new Vector2(-Mathf.Sin(Mathf.Deg2Rad*heading), Mathf.Cos(Mathf.Deg2Rad*heading));

			//get angle between heading and playerToAgent
			float agentAngle = Mathf.Acos(Vector2.Dot(headingVec.normalized, playerToAgent.normalized));
			agentAngle*=Mathf.Rad2Deg;
			if(Vector3.Cross(headingVec, playerToAgent).z > 0){
				agentAngle = 360-agentAngle;
			}

			int j;

			for(j = 0; j < numSlices; j++){
				if((agentAngle < angles[(j+1)%numSlices] && agentAngle >= angles[j])){
					break;
				}
			}
			if(agentAngle >= angles[numSlices-1] || agentAngle < angles[0]){
				j = numSlices-1;
			}

			j = numSlices - j;

			++numAgentsInSlices[j-1];

			if(dispSlices) 
				lableText += (lableText == null?"":"\n") + (j).ToString();

			agentSensors += "Agent at location ("+near[i].renderer.bounds.center.x +", "+ near[i].renderer.bounds.center.y+"):\n  "
				+playerToAgent.magnitude +" units away from player\n  " 
				+(360-agentAngle) +" degrees from heading\n  "
				+"In pie slice " +j +"\n\n"; 

			//Draw labels for pie slices and adjacent agent sensors.
			if(dispAdjAgent || dispSlices) {

				GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
				centeredStyle.alignment = TextAnchor.MiddleCenter;
				GUI.color = Color.white;
				int labelSize = 50;
				GUI.Label(new Rect(pivotAgent.x-(labelSize/2), pivotAgent.y-(labelSize/2), labelSize, labelSize), lableText, centeredStyle);
			}
		}

		string sliceText = "";
		for(int k = 0; k < numSlices; k++) {
			sliceText += "Activation level for slice " + (k+1) +": "+numAgentsInSlices[k] +"\n";
		}
		debugText += sliceText +"\n";

		//Draw debug text for the player
		if (debug) {
			
			debugText = "Coordinates: " +"("+renderer.bounds.center.x +", "+ renderer.bounds.center.y+")" +"\nHeading: " + heading + ".\n\n" + debugText + agentSensors;
			GUI.color = Color.black;
			GUI.Label(new Rect(0, 0, 300, 800), debugText);
		}
	}

	//Draw a box
	private void drawBox(float x, float y, float width, float height, float angle, Vector2 pivot){

		GUIUtility.RotateAroundPivot(angle, pivot);
		GUI.DrawTexture(new Rect(x, y, width, height), lineTexture, ScaleMode.StretchToFill);
		GUIUtility.RotateAroundPivot(-angle, pivot);
	}

}