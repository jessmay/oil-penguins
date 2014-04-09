using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IciclePenguins : Agent {
	
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

	public int health;

	public bool hasPath;

	public int sleepTimer;
	public bool selectable;

	private IciclePenguinFSM IPfsm;

	//Sensor
	public AdjacentAgents adjAgents {get; private set;}
	
	//Initialize the agent
	protected override void initializeAgent(){
		adjAgents = new AdjacentAgents (this, radius * 2, grid);//TODO play around with radius value; smaller than humans
		
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

		health = 100;
		hasPath = false;
		sleepTimer = 0;
		selectable = true;

		IPfsm = new IciclePenguinFSM (this);
	}
	
	//Update agent
	protected override void updateAgent(){
		//Check sensors for adj agents
		sense ();
		aStarUpdate ();
		checkButtons ();
		IPfsm.update ();
	}
	
	//prepare agent for destruction
	//don't do anything, handled in Agent.cs
	protected override void destroyAgent(){}

	public override float getTurnStep() { return turnStep; }
	public override float getMoveStep() { return 10.0f * transform.localScale.x;}//TODO play around with, faster than humans

	//Get information about the environment.
	private void sense() {
		//Get list of nearest agents
		adjAgents.calculate();
	}

	//A* info
	private void aStarUpdate(){

		// If the source node or target node change, the aStar path needs to be updated
		bool sourceChange = false;
		bool targetChange = false;

		//Designate source location; shift + left click
		if (Input.GetKey (KeyCode.LeftShift) && Input.GetMouseButtonDown (0)) {
			Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			
			//if new source location, sets values of source to mouse position
			if((int)pos.x != source.x || (int)pos.y != source.y){
				source = pos;
				sourceCell = grid.getCellIndex(source);
				sourceChange = true;
			}
		}

		//Designate target location; left control+right click
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1)){
			Vector2 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			
			//if new target location, sets values of target to mouse position
			if((int)pos.x != target.x || (int)pos.y != target.y){
				target = pos;
				targetCell = grid.getCellIndex(target);
				targetChange = true;
			}
			
		}

		//if there has been a change in the findTarget value during this call of update
		bool findChange = false;
		//tells the player whether or not to perform A* and seek the target
		if(Input.GetKeyDown(KeyCode.F2)){
			findTarget = !findTarget;
			findChange = true;
		}

		//Get list of close nodes, gets the closest, seeks, and calculates astar path
		if((findChange || targetChange || sourceChange) && findTarget && source != target){
			//Gets list of close nodes within one cell of the agent
			//closeNodes = new List<Vector2>();
			//closeNodes = map.getNearNodes(this);
			
			// Gets the closest node from the list of close nodes and sets it as the source node and the currGoal
			currGoal = new Vector2();
			currGoal = map.getCellIndex(transform.position);
			source = new Vector2();
			source = map.cellIndexToWorld(currGoal);
			
			//gets aStar path if there is one, otherwise turns target seeking off
			if(!aStar(source)){
				findTarget = false;
			}
			else{
				hasPath = true;
			}
			
			pathIndex = 0;
		}

		//Seeks along A* path if findTarget is true
		if(findTarget){
			
			//If the player is at the target, no more need to find the target
			if(map.getCellIndex(renderer.bounds.center) == map.getCellIndex(target)){//distanceBetweenPoint(map.cellIndexToWorld(target)) <= (.5 * transform.localScale.x)
				pathIndex = 0;
				findTarget = false;
				hasPath = false;
			}
			//Otherwise seek towards the current goal location in the aStar path
			else {
				
				//If in the cell index of current goal and not on target, close enough, check next location to seek
				if (distanceBetweenPoint(map.cellIndexToWorld(currGoal)) <= (.5 * transform.localScale.x) && pathIndex < currPath.Count-1)
				{
					pathIndex++;
					currGoal = new Vector2();
					currGoal = (Vector2)currPath[pathIndex];
					seek (map.cellIndexToWorld(currGoal));
				}
				//If not in the cell index of current goal, keep seeking to that current goal
				else if (distanceBetweenPoint(map.cellIndexToWorld(currGoal)) > (.5 * transform.localScale.x))
				{
					seek(map.cellIndexToWorld(currGoal));
				}
			}
		}
	}

	//Check for debug button presses
	private void checkButtons () {
		
		//display adjacent agents
		if(Input.GetKeyDown(KeyCode.C)){
			adjAgents.toggleDisplay();
		}

		// tells the player whether or not to display the source node for debugging purposes
		if (Input.GetKeyDown (KeyCode.U)) {
			drawSource = !drawSource;
		}

		// tells the player whether or not to display the target node for debugging purposes
		if (Input.GetKeyDown (KeyCode.I)) {
			drawTarget = !drawTarget;
		}

		// tells the player whether or not to display the nav graph for debugging purposes
		if (Input.GetKeyDown (KeyCode.O)) {
			drawNodes = !drawNodes;
		}

		// tells the player whether or not to display the A* path for debugging purposes
		if (Input.GetKeyDown (KeyCode.P)) {
			drawPath = !drawPath;
		}

	}
	
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

	protected override bool isControllable(){return false;}
	
	protected override void DrawDebugInformation(){
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

		// Debug code, if there exists an A* path currently and drawPath is true, will draw the nodes of the path on screen
		if (drawPath && currPath != null) {
			for(int i = 0; i < currPath.Count; i++){
				Vector2 node = map.cellIndexToWorld(currPath[i]);
				node.y*=-1;
				DebugRenderer.drawCircle(DebugRenderer.currentCamera.WorldToScreenPoint(node), 5.0f);
				node.y*=-1;
			}
		}

		//Draw circle for nearest agents
		adjAgents.drawSensor();

		//Draw debug text to the screen
		//Get agent information
		string debugText = "Agent Id: "+ gameObject.GetInstanceID() +"\n";
		debugText += "Coordinates: " +"("+renderer.bounds.center.x +", "+ renderer.bounds.center.y+")" +"\n";
		debugText += "Heading: " + heading + ".\n\n";
		
		//Get sensor information
		debugText += adjAgents.getDebugInformation()+ "\n";
		
		GUI.color = Color.black;
		GUI.Label(new Rect(0, 0, 300, 800), debugText);
		
		Vector2 goal = DebugRenderer.currentCamera.WorldToScreenPoint (map.cellIndexToWorld(currGoal));
		DebugRenderer.drawCircle (new Vector2 (goal.x, Screen.height - goal.y), 2 * getRadiusCameraSpace ());
	}
}
