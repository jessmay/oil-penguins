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





	protected Vector2 target;
	protected Vector2 targetCell;
	protected Vector2 source;
	protected Vector2 sourceCell;
	protected bool findTarget;
	
	protected List<Vector2> currPath;
	protected int pathIndex;
	
	protected bool drawSource;
	protected bool drawTarget;
	protected bool drawPath;
	protected bool drawNodes;
	
	protected Vector2 currGoal;
	
	protected List<Vector2> closeNodes;
	protected Vector2 closestN;



	public abstract float getTurnStep();
	public abstract float getMoveStep();
	
	//Initilize the agent
	protected abstract void initializeAgent();

	//Update agent
	protected abstract void updateAgent();

	//prepare agent for destruction
	protected abstract void destroyAgent();

	// Use this for initialization
	void Start() {

		heading = transform.rotation.z;
		velocity = Vector2.zero;
		radius = GetComponent<CircleCollider2D> ().radius * transform.localScale.x;
		gridCellIndex = Vector2.zero;

		//Debug
		//updateCenterInCameraSpace();

		initializeAgent();





		target = new Vector2 (0, 0);
		targetCell = new Vector2();
		source = new Vector2 ();
		sourceCell = new Vector2(0, 0);
		findTarget = false;
		
		drawSource = false;
		drawTarget = false;
		drawPath = false;
		drawNodes = false;
		
		currGoal = map.getCellIndex(renderer.bounds.center);
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
		//updateCenterInCameraSpace();
	}

	//Given a point, returns the distance from the agent to the point.
	public double distanceBetweenPoint (Vector3 point) {
		return Vector2.Distance(renderer.bounds.center, point);
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
			updateCenterInCameraSpace();

			DrawDebugInformation ();






			//Assignment 2
			//Debug code, if drawSource is true, an 'S' will be drawn on the background where the source node is located
			if (drawSource) {
				GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
				centeredStyle.alignment = TextAnchor.MiddleCenter;
				GUI.color = Color.black;
				source.y*=-1; // WorldToScreenPoint inverts the y values for some reason
				int labelSize = 50;
				GUI.Label(new Rect(DebugRenderer.currentCamera.WorldToScreenPoint(source).x-(labelSize/2), DebugRenderer.currentCamera.WorldToScreenPoint(source).y-(labelSize/2),labelSize, labelSize), "S", centeredStyle);
				source.y*=-1;
			}
			
			//Assignment 2
			//Debug code, if drawTarget is true, a 'T' will be drawn on the background where the target node is located
			if (drawTarget)
			{
				GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
				centeredStyle.alignment = TextAnchor.MiddleCenter;
				GUI.color = Color.black;
				target.y*=-1;
				int labelSize = 50;
				//DebugRenderer.drawCircle(DebugRenderer.currentCamera.WorldToScreenPoint(target), 5.0f);
				GUI.Label(new Rect(DebugRenderer.currentCamera.WorldToScreenPoint(target).x-(labelSize/2), DebugRenderer.currentCamera.WorldToScreenPoint(target).y-(labelSize/2),labelSize, labelSize), "T", centeredStyle);
				target.y*=-1;
			}
			
			//Assignment 2
			// Debug code, draws the current moveable nodes in the nav graph onto the screen
			if (drawNodes) {
				for(int i = 0; i < map.getMapWidth(); i++){
					for(int j = 0; j < map.getMapHeight(); j++){
						Vector2 node = map.cellIndexToWorld(new Vector2(i, j));
						node.y*=-1;
						if(map.canMove[i, j]){
							DebugRenderer.drawCircle(DebugRenderer.currentCamera.WorldToScreenPoint(node), 5.0f);
						}
					}
				}
			}
			
			//Assignment 2
			// Debug code, if there exists an A* path currently and drawPath is true, will draw the nodes of the path on screen
			if (drawPath && currPath != null) {
				for(int i = 0; i < currPath.Count; i++){
					Vector2 node = map.cellIndexToWorld(currPath[i]);
					node.y*=-1;
					DebugRenderer.drawCircle(DebugRenderer.currentCamera.WorldToScreenPoint(node), 5.0f);
					node.y*=-1;
				}
			}
		}
	}

	/* 		Debug 		*/
	
	protected bool debug = true;

	private Vector2 centerCameraSpace;

	//Calculates the center of the agent in camera space.
	private void updateCenterInCameraSpace() {

		//Center of player object in local world space
		//Vector3 center = new Vector3(renderer.bounds.center.x, -renderer.bounds.center.y);
		
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













	//Assignment 2
	// Not using a priority queue, using a hash table, so have to find the node with the min f value each round
	private Vector2 findMin(Hashtable l, List<Vector2> curr){
		float min = -1.0f;
		Vector2 minVec = new Vector2 (0, 0);
		
		foreach (DictionaryEntry d in l) {
			if((((float)d.Value) < min || min == -1) && curr.Contains((Vector2)d.Key)){
				min = (float)d.Value;
				minVec = (Vector2)d.Key;
			}
		}
		
		return minVec;
	}

	// A* algorithm
	// Uses map cells to detect where walls are
	// If there is a path, it will be reconstructed at the end of this function
	// and it will contain the locations of nodes in Map Cell space to follow
	// Source node is in world coords
	protected bool aStar(Vector2 sourceNode){
		
		Vector2 mapCellIndex = map.getCellIndex (sourceNode);
		
		List<Vector2> visited = new List<Vector2> ();
		List<Vector2> curr = new List<Vector2> ();
		curr.Add (mapCellIndex);
		Hashtable from = new Hashtable ();
		
		Hashtable gVals = new Hashtable ();
		gVals.Add (mapCellIndex, 0.0f);
		Hashtable fVals = new Hashtable ();
		fVals.Add (mapCellIndex, ((float)gVals [mapCellIndex]) + Vector2.Distance (mapCellIndex, map.getCellIndex(target)));
		
		while (curr.Count != 0) {
			
			Vector2 currNode = findMin(fVals, curr);
			
			// Arrived at the target? Great! Make the path list.
			if(currNode == map.getCellIndex(target)){
				//reconstruct path with the from list
				currPath = new List<Vector2>();
				currPath = createPath(from, currNode);
				
				return true; 
			}
			
			curr.Remove(currNode);
			visited.Add(currNode);
			
			//for each neighbor node, add to curr if unvisited and calculate f and g values
			for(int i = -1; i < 2; i++){
				for(int j = -1; j < 2; j++){
					
					int x = (int)currNode.x+i;
					int y = (int)currNode.y+j;
					
					//currnode not inbounds, or not moveable to
					if(!map.inBounds(new Vector2(x, y)) || !map.canMove[x,y])
						continue;
					
					//corner case:if node to move to is diagonal, but perp nodes are nonmoveable, can't go there
					if(Mathf.Abs(i) == Mathf.Abs(j) && (!map.canMove[x,(int)currNode.y] || !map.canMove[(int)currNode.x,y])){
						continue;
					}
					
					Vector2 neighbor = new Vector2(x, y);
					if(visited.Contains(neighbor)){//also covers 0,0 case
						continue;
					}
					
					float tempG = (float)gVals[currNode] + Vector2.Distance(currNode, neighbor);
					
					if(!curr.Contains(neighbor) || tempG < (float)gVals[currNode]){
						
						from.Add(neighbor, currNode);
						gVals.Add(neighbor, tempG);
						fVals.Add(neighbor, tempG + Vector2.Distance(neighbor, map.getCellIndex(target)));
						
						if(!curr.Contains(neighbor))
							curr.Add(neighbor);
					}
				}
			}
		}
		return false;
	}
	
	
	// If aStar() finds a path, this function creates a List for that path
	// that the agent can then access
	private List<Vector2> createPath(Hashtable path,  Vector2 node){
		
		if (path.Contains (node)) {
			List<Vector2> p = new List<Vector2>();
			p = createPath(path, (Vector2)path[node]);
			p.Add(node);
			return p;
		}
		else{
			List<Vector2> p = new List<Vector2>();
			p.Add(node);
			return p;
		}
		
	}
	
	//returns the closest unobstructed node in the navgraph
	//the node coordinates will be in map cell space
	protected Vector2 closestUnobstructed(){
		
		Vector2 cNode = closeNodes[0];
		float minDist = Vector2.Distance (cNode, renderer.bounds.center);
		
		for (int i = 1; i < closeNodes.Count; i++) {
			Vector2 currNode = closeNodes[i];
			
			if(Vector2.Distance(currNode, renderer.bounds.center) < minDist){
				cNode = currNode;
				minDist = Vector2.Distance(cNode, renderer.bounds.center);
			}
		}
		
		return cNode;
	}

}
