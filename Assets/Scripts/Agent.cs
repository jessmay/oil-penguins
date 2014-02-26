/*
Jessica May
Joshua Linge
Agent.cs
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour {

	public Background background;

	protected float heading;
	protected Vector2 velocity;

	protected Vector2 cellIndex;

	protected float radius;

	protected int viewAngle = 180;
	
	protected bool dispFeelers = false;
	protected int numFeelers = 3;
	protected Vector2[] feelers;
	protected int feelerLength;
	
	protected bool dispAdjAgent = false;
	protected int adjRadius;
	public Texture circle;
	protected Texture adjCircle;
	
	protected bool dispSlices = false;
	protected int numSlices = 4;
	protected int pieAngle = 360;
	protected int[] sliceAngles;
	protected List<Agent>[] slicesOfAgents;
	
	protected List<Agent> near;
	
	protected float lineWidth = .5f;
	
	public Texture lineTexture;
	
	protected bool debug = true;
	
	private float turnStep = 5.0f;
	private float moveStep = 10.0f;

	
	// Use this for initialization
	protected void Start() {
		heading = transform.rotation.z;
		velocity = Vector2.zero;
		radius = GetComponent<CircleCollider2D> ().radius;
		cellIndex = Vector2.zero;
	}

	protected void moveForward() {
		Vector2 temp = new Vector2(transform.up.x, transform.up.y);
		velocity = temp * (float)moveStep;
	}

	protected void moveBackward() {
		Vector2 temp = new Vector2(transform.up.x, transform.up.y);
		velocity = temp * (float)-moveStep;
	}
	
	protected void lookRight() {
		heading-=turnStep;
		if(heading < 0) heading = (heading+360)%360;
		transform.rotation *= Quaternion.Euler (0,0,-turnStep);
	}
	
	protected void lookLeft() {
		heading+=turnStep;
		if(heading >= 360) heading%=360;
		transform.rotation *= Quaternion.Euler (0,0, turnStep);
	}

	protected void Move() {
		Vector2 prevCell = cellIndex;

		rigidbody2D.velocity = velocity; 
		velocity = Vector2.zero;

		cellIndex = background.grid.getCellIndex(renderer.bounds.center);
		if (cellIndex != prevCell) {
			background.grid.move(this, prevCell, cellIndex);
		}
	}

	protected void getLengthOfFeelers(int range, int amount, int viewAngle = 180) {
		
		// Save current object layer
		int oldLayer = gameObject.layer;
		
		//Change object layer to a layer it will be alone
		gameObject.layer = LayerMask.NameToLayer("RayCast");
		
		int layerToIgnore = 1 << gameObject.layer;
		layerToIgnore = ~layerToIgnore;
		
		
		feelers =  new Vector2[amount];
		float spaceBetween = viewAngle/(amount+1);
		
		for (int currentFeeler = 0; currentFeeler < amount; ++currentFeeler) {
			
			int angle = (int) (heading - viewAngle/2 + spaceBetween*(currentFeeler+1));
			
			angle = (angle + 360) % 360;
			
			Vector2 direction = new Vector2(-Mathf.Sin(Mathf.Deg2Rad*angle), Mathf.Cos(Mathf.Deg2Rad*angle));
			RaycastHit2D hit = Physics2D.Raycast((Vector2)(renderer.bounds.center) + direction * radius, direction.normalized, range, layerToIgnore);
			feelers[currentFeeler] = hit.collider == null? direction*range: hit.fraction * direction*range;
			
			Debug.DrawRay((Vector2)(renderer.bounds.center) + direction * radius, feelers[currentFeeler], Color.black);
		}
		
		// set the game object back to its original layer
		gameObject.layer = oldLayer;
		
	}
	
	protected void getPieSliceAngles(){
		sliceAngles = new int[numSlices];
		
		float spaceBetween = pieAngle/numSlices;
		
		for(int currSlice = 0; currSlice < numSlices; currSlice++){
			sliceAngles[currSlice] = (int)((spaceBetween/2)+(currSlice*spaceBetween));
		}
	}

	public Vector2 getCellIndex () {
		return cellIndex;
	}

	public float getRadius () {
		return radius;
	}

	protected void DrawOnGUI(){
		
		//string debugText = "";
		//string agentSensors = "";
		
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
		
		// Draw feelers
		if (dispFeelers) {
			drawFeelers (pivot, radiusV, width);
		}
		
		
		//Draw circle for nearest agents
		if (dispAdjAgent) {
			drawAdjAgent(pivot, radiusV, width, length);
		}
		
		//Draw pie slices
		if (dispSlices){
			drawPieSlices(pivot, radiusV, width,length);
		}

		if (dispSlices || dispAdjAgent) {
			drawAgentLabels();
		}
		
		/*string sliceText = "";
		for(int k = 0; k < numSlices; k++) {
			sliceText += "Activation level for slice " + (k+1) +": "+numAgentsInSlices[k] +"\n";
		}
		debugText += sliceText +"\n";*/
		
		//Draw debug text for the player
		/*if (debug) {
			
			debugText = "Coordinates: " +"("+renderer.bounds.center.x +", "+ renderer.bounds.center.y+")" +"\nHeading: " + heading + ".\n\n" + debugText + agentSensors;
			GUI.color = Color.black;
			GUI.Label(new Rect(0, 0, 300, 800), debugText);
		}*/
	}

	//Draw agent labels
	protected void drawAgentLabels(){
		for (int i = 0; i < numSlices; i++) {

			for(int j = 0; j < slicesOfAgents[i].Count; j++){

				string lableText = null;
				
				if (dispAdjAgent) 
					lableText = "Within";

				Vector3 currAgent = new Vector3(slicesOfAgents[i][j].renderer.bounds.center.x, slicesOfAgents[i][j].renderer.bounds.center.y);
				currAgent.Scale(new Vector3(1, -1, 1));
				Vector2 pivotAgent = (Vector2)Camera.main.WorldToScreenPoint(currAgent);

				if(dispSlices) 
					lableText += (lableText == null?"":"\n") + (i+1).ToString();

							
				GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
				centeredStyle.alignment = TextAnchor.MiddleCenter;
				GUI.color = Color.white;
				int labelSize = 50;
				GUI.Label(new Rect(pivotAgent.x-(labelSize/2), pivotAgent.y-(labelSize/2), labelSize, labelSize), lableText, centeredStyle);
			}

		}
	}

	//Draws the agents feelers given the number of feelers
	protected void drawFeelers(Vector2 pivot, float radiusV, Vector2 width){
		float spaceBetween = viewAngle/(feelers.Length+1);
		for (int currentFeeler = 0; currentFeeler < feelers.Length; ++currentFeeler) {
			
			int angle = (int) (heading - viewAngle/2 + spaceBetween*(currentFeeler+1));
			angle = (angle + 360) % 360;
			
			Vector2 feelerVec = new Vector2(feelers[currentFeeler].magnitude, 0);
			feelerVec = (Camera.main.WorldToScreenPoint(feelerVec) - Camera.main.WorldToScreenPoint(Vector2.zero));
			
			drawBox (pivot.x-width.x/2, pivot.y+radiusV, width.x, feelerVec.x, -angle+180, pivot);
			
			//debugText += "Feeler["+currentFeeler+"]: "+ feelers[currentFeeler].magnitude +"\n";
		}
		//debugText += "\n";
	}
	
	//Draws adjacent agent circle based on a specified radius range
	protected void drawAdjAgent(Vector2 pivot, float radiusV, Vector2 width, Vector2 length){
		float circleSize = 2*(width.x + length.x + radiusV);
		GUI.DrawTexture(new Rect(pivot.x-circleSize/2, pivot.y-circleSize/2, circleSize, circleSize), circle, ScaleMode.ScaleToFit);
	}
	
	//Draw pie slices
	protected void drawPieSlices(Vector2 pivot, float radiusV, Vector2 width, Vector2 length){
		for (int i = 0; i < numSlices; i++) {
			drawBox (pivot.x-width.x/2, pivot.y+radiusV, width.x, length.x, -heading+sliceAngles[i], pivot);
		}
	}
	
	//Gets the agents within the pie slices
	protected void findPieSlices(){
		int[] numAgentsInSlices = new int[numSlices];
		
		slicesOfAgents = new List<Agent>[numSlices];
		for(int i = 0; i < numSlices; i++){
			slicesOfAgents[i] = new List<Agent>();
		}
		
		//Draw lable for each agent within a pie slice or circle
		for(int i = 0; i < near.Count; i++){
			Vector3 currAgent = new Vector3(near[i].renderer.bounds.center.x, near[i].renderer.bounds.center.y);
			currAgent.Scale(new Vector3(1, -1, 1));
			//Vector2 pivotAgent = (Vector2)Camera.main.WorldToScreenPoint(currAgent);
			
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
				if((agentAngle < sliceAngles[(j+1)%numSlices] && agentAngle >= sliceAngles[j])){
					break;
				}
			}
			if(agentAngle >= sliceAngles[numSlices-1] || agentAngle < sliceAngles[0]){
				j = numSlices-1;
			}
			
			j = numSlices - j;
			
			++numAgentsInSlices[j-1];
			
			slicesOfAgents[j-1].Add(near[i]);
			
		}
	}
	
	//Draw a box
	protected void drawBox(float x, float y, float width, float height, float angle, Vector2 pivot){
		
		GUIUtility.RotateAroundPivot(angle, pivot);
		GUI.DrawTexture(new Rect(x, y, width, height), lineTexture, ScaleMode.StretchToFill);
		GUIUtility.RotateAroundPivot(-angle, pivot);
	}
}
