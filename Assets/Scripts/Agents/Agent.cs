/*
Jessica May
Joshua Linge
Agent.cs

Updated by Joshua Linge on 2014-03-17
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Agent : MonoBehaviour {

	public Grid grid;
	public Map map;

	protected float radius;
	protected float heading;
	protected Vector2 velocity;

	protected Vector2 gridCellIndex;

	protected float turnStep = 5.0f;
	protected float moveStep = 10.0f;

	public abstract float getTurnStep();
	public abstract float getMoveStep();
	
	//Initialize the agent
	protected abstract void initializeAgent();

	//Update agent
	protected abstract void updateAgent();

	//prepare agent for destruction
	protected abstract void destroyAgent();

	// Use this for initialization
	void Start() {

		heading = transform.rotation.eulerAngles.z;
		velocity = Vector2.zero;
		radius = GetComponent<CircleCollider2D> ().radius * transform.localScale.x;
		gridCellIndex = Vector2.zero;

		//Debug
		updateCenterInCameraSpace();

		initializeAgent();
	}

	void FixedUpdate() {

		checkControls();

		updateAgent();

		Move();
	}

	void OnDestroy() {

		destroyAgent ();

		//Remove agent from the grid
		grid.remove(this);
	}

	public static GameObject CreateAgent(GameObject agent, Vector3 location, Quaternion rotation, Map map, Grid grid) {

		GameObject newAgent = Instantiate(agent, location, rotation) as GameObject;
		Agent agentScript = newAgent.GetComponent<Agent>();
		
		agentScript.map = map;
		agentScript.grid = grid;

		return newAgent;
	}
	
	//Get the cell index into the agent grid.
	public Vector2 getGridCellIndex () {
		return gridCellIndex;
	}

	//Get the radius of this agent.
	public float getRadius () {
		return radius;
	}

	//Get the heading of this agent.
	public float getHeading() {
		return heading;
	}

	//Seek to the desired location.
	public void seek (Vector2 target, bool arrive = false) {

		//Get the angle to the target.
		double angle = getAngleToPoint(target);

		//Get the distance to the target.
		double distance = distanceBetweenPoint(target);

		//Turn to the target if not facing.
		if(Math.Abs(angle) > .0) {
			turn(Mathf.Clamp((float)angle, -getTurnStep(), getTurnStep()));
			if (Math.Abs(angle) > 30)
				return;
		}

		//If far from the target, move towards the target.
		if (distance > .1 * transform.localScale.x) {//

			//If arrive, slow the approach as the agent gets closer to the target.
			if(arrive)
				moveTo(Mathf.Clamp((float)(distance), 0.0f, getMoveStep()));

			//Else, move to target at set speed.
			else
				moveTo (getMoveStep());
			return;
		}
	}

	//Update the agent's velocity by the given move amount.
	public void moveTo(float moveStep) {
		Vector2 temp = new Vector2(transform.up.x, transform.up.y);
		velocity = temp * moveStep;
	}

	//Rotate the agent by the given angle (in degrees).
	public void turn(float turnStep) {
		heading+=turnStep;
		if(heading >= 360) heading%=360;
		if(heading < 0) heading = (heading+360)%360;
		transform.rotation *= Quaternion.Euler (0,0, turnStep);
	}

	//Move the agent based on the current velocity.
	private void Move() {

		//Do not move outside the bounds of the grid.
//		if(!grid.inBounds(grid.getCellIndex(velocity*Time.deltaTime + (Vector2)transform.position))) {
//			velocity = Vector2.zero; 
//		}

		//Force agents to be within the bounds of the map.
		Vector3 clampedPos = transform.position;
		Bounds mapBounds = map.getBounds();

		clampedPos.x = Mathf.Clamp(clampedPos.x, mapBounds.min.x, mapBounds.max.x);
		clampedPos.y = Mathf.Clamp(clampedPos.y, mapBounds.min.y, mapBounds.max.y);

		transform.position = clampedPos;

		//Save the previous grid cell index.
		Vector2 prevCell = gridCellIndex;

		//Update agent's physics component.
		rigidbody2D.velocity = velocity; 
		velocity = Vector2.zero;

		//Get new grid cell index.
		gridCellIndex = grid.getCellIndex(renderer.bounds.center);

		//Update the grid if the indecies are different.
		if (gridCellIndex != prevCell) {
			grid.move(this, prevCell, gridCellIndex);
		}

		//Update debug information
		updateCenterInCameraSpace();
	}

	//Given a point, returns the distance from the agent to the point.
	public double distanceBetweenPoint (Vector3 point) {
		return Vector2.Distance(transform.position, point);
	}

	//Given a point, returns the angle between the agent's heading vector 
	// and the vector from the agent to the point.
	public double getAngleToPoint(Vector3 point) {
		
		Vector2 toTarget = (point - transform.renderer.bounds.center).normalized;
		Vector2 headingVec = new Vector2(-Mathf.Sin(Mathf.Deg2Rad*heading), Mathf.Cos(Mathf.Deg2Rad*heading));
		
		double angle = Vector2.Angle(headingVec, toTarget);
		if(Vector3.Cross(headingVec, toTarget).z < 0){
			angle = -angle;
		}
		
		return angle;
	}

	//Draw debug information.
	void OnGUI(){

		if(debug) {
			DrawDebugInformation ();
		}
	}

	/* 		Debug 		*/
	
	protected bool debug = true;

	private Vector2 centerCameraSpace;

	//Calculates the center of the agent in camera space.
	private void updateCenterInCameraSpace() {

		//Player's center in camera space
		centerCameraSpace = (Vector2)DebugRenderer.currentCamera.WorldToScreenPoint(renderer.bounds.center);
		centerCameraSpace.y = Screen.height - centerCameraSpace.y;
	}

	//Gets the center of this agent in camera space coordinates.
	public Vector2 getCenterCameraSpace() {
		return centerCameraSpace;
	}

	//Gets the radius of this agent in camera space coordinates.
	public float getRadiusCameraSpace() {
		return DebugRenderer.worldToCameraLength(radius);// * transform.localScale.x
	}

	//Draw debug information to the screen.
	protected abstract void DrawDebugInformation();


	/* 		Debug controls 		*/

	//Is this agent controllable?
	protected abstract bool isControllable();

	//Check debug controls and update accordingly.
	private void checkControls () {

		//Display debug text
		if(Input.GetKeyDown(KeyCode.F1)) {
			debug = !debug;
		}

		//If it is possible for the user to control this agent
		if(isControllable()){

			//Turn counter clockwise
			if (Input.GetKey(KeyCode.LeftArrow)) {
				turn(getTurnStep());
			}
			
			//Turn clockwise
			if (Input.GetKey(KeyCode.RightArrow)) {
				turn(-getTurnStep());
			}
			
			//Move forward
			if (Input.GetKey(KeyCode.UpArrow)) {
				moveTo(getMoveStep());
			}
			
			//Move backward
			if (Input.GetKey(KeyCode.DownArrow)) {
				moveTo(-getMoveStep());
			}
		}
	}
}
