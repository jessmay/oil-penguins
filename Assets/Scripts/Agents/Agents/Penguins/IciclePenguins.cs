using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IciclePenguins : GameAgent {
	
	/*protected Vector2 target;
	protected Vector2 targetCell;
	protected Vector2 source;
	protected Vector2 sourceCell;*/
	protected bool findTarget;
	
	//protected List<Vector2> currPath;
	protected int pathIndex;
	
	protected bool drawSource;
	protected bool drawTarget;
	protected bool drawPath;
	protected bool drawNodes;
	
	protected Vector2 currGoal;

	//public int health;
	protected override float getMaxHealth () {
		return 100;
	}

	public bool hasPath;

	public int sleepTimer;
	public bool selectable;

	private AStar aStar;

	private IciclePenguinFSM IPfsm;

	//Sensor
	public AdjacentAgents adjAgents {get; private set;}
	
	//Initialize the agent
	protected override void initializeAgent(){
		base.initializeAgent();

		adjAgents = new AdjacentAgents (this, radius * 2, grid);//TODO play around with radius value; smaller than humans

		aStar = new AStar (map);
		/*target = new Vector2 (0, 0);
		targetCell = new Vector2();
		source = new Vector2 ();
		sourceCell = new Vector2(0, 0);*/
		findTarget = false;
		
		drawSource = false;
		drawTarget = false;
		drawPath = false;
		drawNodes = false;
		
		currGoal = map.getCellIndex(transform.position);

		//health = 100;
		hasPath = false;
		sleepTimer = 0;
		selectable = true;

		IPfsm = new IciclePenguinFSM (this);
	}
	
	//Update agent
	protected override void updateAgent(){
		base.updateAgent();
		//updates A*
		aStarUpdate ();

		//if health equals 0, then sleep
		if (IPfsm.penguin.health == 0) {
			//change state to sleep
			IPfsm.changeState(typeof(IciclePenguinSleepState));
		}
		else if (aStar.hasPath) {
			IPfsm.changeState(typeof(IciclePenguinMoveState));
		}

		//Check sensors for adj agents
		sense ();



		//checkButtons ();
		IPfsm.update ();
	}
	
	//prepare agent for destruction
	//don't do anything, handled in Agent.cs
	protected override void destroyAgent(){
		gameMap.PenguinsOnMap.Remove (gameObject);
	}


	public override void onDeath () {
		//change state to sleep
		IPfsm.changeState(typeof(IciclePenguinSleepState));
	}


	public override float getTurnStep() { return turnStep; }
	public override float getMoveStep() { return 10.0f * transform.localScale.x;}//TODO play around with, faster than humans


	//Render non debug information here
	protected override void drawStatus () {
		base.drawStatus();
	}

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
			if((int)pos.x != aStar.source.x || (int)pos.y != aStar.source.y){
				aStar.setSource(pos);
				sourceChange = true;
			}
		}

		//Designate target location; left control+right click
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1)){
			Vector2 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			
			//if new target location, sets values of target to mouse position
			if((int)pos.x != aStar.target.x || (int)pos.y != aStar.target.y){
				aStar.setTarget(pos);
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
		if((findChange || targetChange || sourceChange) && findTarget && aStar.source != aStar.target){
			
			// Gets the closest node from the list of close nodes and sets it as the source node and the currGoal
			currGoal = new Vector2();
			currGoal = map.getCellIndex(transform.position);
			aStar.setSource(map.cellIndexToWorld(currGoal));//source = new Vector2();
			//source = map.cellIndexToWorld(currGoal);
			
			//gets aStar path if there is one, otherwise turns target seeking off
			if(!aStar.aStar()){
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
			if(map.getCellIndex(transform.position) == aStar.targetCell){//distanceBetweenPoint(map.cellIndexToWorld(target)) <= (.5 * transform.localScale.x)
				pathIndex = 0;
				findTarget = false;
				hasPath = false;
			}
			//Otherwise seek towards the current goal location in the aStar path
			else {
				
				//If in the cell index of current goal and not on target, close enough, check next location to seek
				if (distanceBetweenPoint(map.cellIndexToWorld(currGoal)) <= (.5 * transform.localScale.x) && pathIndex < aStar.currPath.Count-1)
				{
					pathIndex++;
					currGoal = new Vector2();
					currGoal = (Vector2)aStar.currPath[pathIndex];
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

	//Check for button presses (or mouse clicks) here

	//Check for debug button presses
	protected override void checkButtons () {
		
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

	protected override bool isControllable(){return false;}
	
	protected override void DrawDebugInformation(){
		//Debug code, if drawSource is true, an 'S' will be drawn on the background where the source node is located
		if (drawSource) {
			GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
			centeredStyle.alignment = TextAnchor.MiddleCenter;
			GUI.color = Color.black;
			aStar.source.y*=-1; // WorldToScreenPoint inverts the y values for some reason
			int labelSize = 50;
			GUI.Label(new Rect(DebugRenderer.currentCamera.WorldToScreenPoint(aStar.source).x-(labelSize/2), DebugRenderer.currentCamera.WorldToScreenPoint(aStar.source).y-(labelSize/2),labelSize, labelSize), "S", centeredStyle);
			aStar.source.y*=-1;
		}

		//Debug code, if drawTarget is true, a 'T' will be drawn on the background where the target node is located
		if (drawTarget)
		{
			GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
			centeredStyle.alignment = TextAnchor.MiddleCenter;
			GUI.color = Color.black;
			aStar.target.y*=-1;
			int labelSize = 50;
			//DebugRenderer.drawCircle(DebugRenderer.currentCamera.WorldToScreenPoint(target), 5.0f);
			GUI.Label(new Rect(DebugRenderer.currentCamera.WorldToScreenPoint(aStar.target).x-(labelSize/2), DebugRenderer.currentCamera.WorldToScreenPoint(aStar.target).y-(labelSize/2),labelSize, labelSize), "T", centeredStyle);
			aStar.target.y*=-1;
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
		if (drawPath && aStar.currPath != null) {
			for(int i = 0; i < aStar.currPath.Count; i++){
				Vector2 node = map.cellIndexToWorld(aStar.currPath[i]);
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
		debugText += "Coordinates: " +"("+transform.position.x +", "+ transform.position.y+")" +"\n";
		debugText += "Heading: " + heading + ".\n\n";
		
		//Get sensor information
		debugText += adjAgents.getDebugInformation()+ "\n";
		
		GUI.color = Color.black;
		GUI.Label(new Rect(0, 0, 300, 800), debugText);
		
		Vector2 goal = DebugRenderer.currentCamera.WorldToScreenPoint (map.cellIndexToWorld(currGoal));
		DebugRenderer.drawCircle (new Vector2 (goal.x, Screen.height - goal.y), 2 * getRadiusCameraSpace ());
	}
}
