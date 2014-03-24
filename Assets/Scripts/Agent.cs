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

	// Use this for initialization
	protected void Start() {
		heading = transform.rotation.z;
		velocity = Vector2.zero;
		radius = GetComponent<CircleCollider2D> ().radius;
		cellIndex = Vector2.zero;

		target = new Vector2 (0, 0);
		targetCell = new Vector2();
		source = new Vector2 ();
		sourceCell = new Vector2(0, 0);
		findTarget = false;

		drawSource = false;
		drawTarget = false;
		drawPath = false;
		drawNodes = false;

        currGoal = background.map.getCellIndex(renderer.bounds.center);
	}

	//Assignment 2 Seek
	//turns agent towards the current goal node
	protected void seek(){

		//calculate new heading based on current heading and target location
		Vector2 agentToTarget = (Vector2)background.map.cellIndexToWorld(currGoal)- (Vector2)renderer.bounds.center;
		Vector2 headingVec = new Vector2(-Mathf.Sin(Mathf.Deg2Rad*heading), Mathf.Cos(Mathf.Deg2Rad*heading));

		//get angle between agent heading and target
		float targetAngle = Mathf.Acos(Vector2.Dot(headingVec.normalized, agentToTarget.normalized));

		//set new heading
		if (targetAngle > .5) {
			turn (Mathf.Rad2Deg*targetAngle);
		}
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

	//Assignment 2
	// Turns the agent to the given angle
	protected void turn(float angle){
		heading-=angle;
		if(heading < 0) heading = (heading+360)%360;
		transform.rotation *= Quaternion.Euler (0,0,-angle);
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

		//Assignment 2
		//Debug code, if drawSource is true, an 'S' will be drawn on the background where the source node is located
		if (drawSource) {
			GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
			centeredStyle.alignment = TextAnchor.MiddleCenter;
			GUI.color = Color.black;
            source.y*=-1; // WorldToScreenPoint inverts the y values for some reason
			int labelSize = 50;
			GUI.Label(new Rect(Camera.main.WorldToScreenPoint(source).x-(labelSize/2), Camera.main.WorldToScreenPoint(source).y-(labelSize/2),labelSize, labelSize), "S", centeredStyle);
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
			//DebugRenderer.drawCircle(Camera.main.WorldToScreenPoint(target), 5.0f);
			GUI.Label(new Rect(Camera.main.WorldToScreenPoint(target).x-(labelSize/2), Camera.main.WorldToScreenPoint(target).y-(labelSize/2),labelSize, labelSize), "T", centeredStyle);
			target.y*=-1;
		}

		//Assignment 2
		// Debug code, draws the current moveable nodes in the nav graph onto the screen
		if (drawNodes) {
			for(int i = 0; i < background.map.getMapWidth(); i++){
				for(int j = 0; j < background.map.getMapHeight(); j++){
					Vector2 node = background.map.cellIndexToWorld(new Vector2(i, j));
					node.y*=-1;
					if(background.map.canMove[i, j]){
						DebugRenderer.drawCircle(Camera.main.WorldToScreenPoint(node), 5.0f);
					}
				}
			}
		}

		//Assignment 2
		// Debug code, if there exists an A* path currently and drawPath is true, will draw the nodes of the path on screen
		if (drawPath && currPath != null) {
			for(int i = 0; i < currPath.Count; i++){
                Vector2 node = background.map.cellIndexToWorld(currPath[i]);
                node.y*=-1;
                DebugRenderer.drawCircle(Camera.main.WorldToScreenPoint(node), 5.0f);
                node.y*=-1;
            }
        }
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
		}
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

	public Vector2 getTarget(){
		return target;
	}

	public void setTarget(Vector2 t){
		target = t;
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

		Vector2 mapCellIndex = background.map.getCellIndex (sourceNode);

		List<Vector2> visited = new List<Vector2> ();
		List<Vector2> curr = new List<Vector2> ();
		curr.Add (mapCellIndex);
		Hashtable from = new Hashtable ();

		Hashtable gVals = new Hashtable ();
		gVals.Add (mapCellIndex, 0.0f);
		Hashtable fVals = new Hashtable ();
		fVals.Add (mapCellIndex, ((float)gVals [mapCellIndex]) + Vector2.Distance (mapCellIndex, background.map.getCellIndex(target)));

		while (curr.Count != 0) {

			Vector2 currNode = findMin(fVals, curr);

			// Arrived at the target? Great! Make the path list.
			if(currNode == background.map.getCellIndex(target)){
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
					if(!background.map.inBounds(new Vector2(x, y)) || !background.map.canMove[x,y])
						continue;

					//corner case:if node to move to is diagonal, but perp nodes are nonmoveable, can't go there
					if(Mathf.Abs(i) == Mathf.Abs(j) && !background.map.canMove[x,(int)currNode.y] && !background.map.canMove[(int)currNode.x,y]){
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
						fVals.Add(neighbor, tempG + Vector2.Distance(neighbor, background.map.getCellIndex(target)));

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
