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

	protected float radius;
	protected float heading;
	protected Vector2 velocity;

	protected Vector2 cellIndex;

	
	protected bool dispFeelers = false;
	protected Feelers feelers;
	
	protected bool dispAdjAgent = false;
	protected AdjcentAgents adjAgents;

	protected bool dispSlices = false;
	protected PieSlices pieSlices;

	protected bool debug = true;
	
	private float turnStep = 5.0f;
	private float moveStep = 10.0f;

	
	public Vector2 getCellIndex () {
		return cellIndex;
	}
	
	public float getRadius () {
		return radius;
	}
	
	public float getHeading() {
		return heading;
	}

	// Use this for initialization
	protected void Start() {

		heading = transform.rotation.z;
		velocity = Vector2.zero;
		radius = GetComponent<CircleCollider2D> ().radius;
		cellIndex = Vector2.zero;

		//Debug
		radiusCameraSpace = DebugRenderer.worldToCameraLength(radius);
		updateCenterInCameraSpace();

		//Sensors
		feelers = new Feelers(this, radius*3);
		adjAgents = new AdjcentAgents(this, radius*3, background.grid);
		pieSlices = new PieSlices(this, adjAgents);
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

		updateCenterInCameraSpace();
	}


	/* 		Debug 		*/
	private Vector2 centerCameraSpace;
	private float radiusCameraSpace;

	private void updateCenterInCameraSpace() {

		//Center of player object in local world space
		Vector3 center = new Vector3(renderer.bounds.center.x, -renderer.bounds.center.y);
		
		//Player's center in camera space
		centerCameraSpace = (Vector2)Camera.main.WorldToScreenPoint(center);
	}

	public Vector2 getCenterCameraSpace() {
		return centerCameraSpace;
	}

	public float getRadiusCameraSpace() {
		return radiusCameraSpace;
	}
	
	protected void DrawDebugInformation(){
		
		// Draw feelers
		if (dispFeelers) {
			feelers.drawDebugInformation();
		}
		
		//Draw circle for nearest agents
		if (dispAdjAgent) {
			adjAgents.drawDebugInformation();
		}
		
		//Draw pie slices
		if (dispSlices){
			pieSlices.drawDebugInformation();
		}
		
//		if (dispSlices || dispAdjAgent) {
//			drawAgentLabels();
//		}
		
		//Draw debug text to the screen
		if (debug) {
			
			//Get agent information
			string debugText = "Agent Id: "+ gameObject.GetInstanceID() +"\n";
			debugText += "Coordinates: " +"("+renderer.bounds.center.x +", "+ renderer.bounds.center.y+")" +"\n";
			debugText += "Heading: " + heading + ".\n\n";
			
			//Get sensor information
			debugText += feelers.getDebugInformation()+ "\n";
			debugText += adjAgents.getDebugInformation()+ "\n";
			debugText += pieSlices.getDebugInformation()+ "\n";
			
			GUI.color = Color.black;
			GUI.Label(new Rect(0, 0, 300, 800), debugText);
		}
	}

	/*
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
	*/
}
