using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class IciclePenguins : GameAgent {
	
	protected bool drawSource;
	protected bool drawTarget;
	protected bool drawPath;
	protected bool drawNodes;

	private bool selected;

	public Sprite[] penguinSprites;

	//public int health;
	protected override float getMaxHealth () {
		return 100;
	}

	//public bool hasPath;
	
	public bool selectable;

	public AStar aStar;

	public IciclePenguinFSM IPfsm {get; private set;}

	//Sensor
	public AdjacentAgents adjAgents {get; private set;}
	

	//Initialize the agent
	protected override void initializeAgent(){
		base.initializeAgent();

		adjAgents = new AdjacentAgents (this, radius * 2, grid, typeof(HumanAgent));//TODO play around with radius value; smaller than humans

		aStar = new AStar (this);
		
		drawSource = false;
		drawTarget = false;
		drawPath = false;
		drawNodes = false;

		selectable = true;
		selected = false;

		IPfsm = new IciclePenguinFSM (this);
	}
	
	//Update agent
	protected override void updateAgent(){
		base.updateAgent();

		if (IPfsm.currentState.GetType() != typeof(IciclePenguinSleepState) 
		    	&& IPfsm.currentState.GetType() != typeof(IciclePenguinMoveState) 
		    	&& aStar.hasPath) {
			IPfsm.changeState(typeof(IciclePenguinMoveState));
		}

		if(selected && Input.GetMouseButtonDown (0)){
			//Position of the second mouse click
			Vector2 pos = DebugRenderer.currentCamera.ScreenToWorldPoint (Input.mousePosition);
			
			// if there is an AStar path, FSM will change state to move state
			// otherwise the penguin will stay in its current state
			aStar.setSource (transform.position);
			aStar.setTarget (pos);
			aStar.aStar();

			selected = false;
        }

		//Check sensors for adj agents
		sense ();
		getClosestAttackable ();

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
		health.restoreToFullHealth();
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

	// TODO start of the selection stuff
	// When an individual is clicked, it is selected using this function
	void OnMouseUpAsButton(){
		selected = true;
	}

	private void getClosestAttackable(){
		//Find closest unobstructed human
		Agent closestAgent = null;
		float distance = float.MaxValue;
		
		//Loop though all agents within range
		foreach(Agent agent in adjAgents.near) {
			
			//Calculate the vector between this penguin and the human.
			Vector2 direction = agent.transform.position - transform.position;
			
			//if the human is not closer than what has already been found, ignore.
			if(distance < direction.magnitude)
				continue;
			
			//Raycast to see if there is line of sight to the human.
			RaycastHit2D rayCastHit = Physics2D.Raycast((Vector2)transform.position + (direction.normalized * radius * 1.01f), direction.normalized, direction.magnitude);
			
			//If the object found is the same as the human we are considering, set as the current closest agent.
			if(rayCastHit.collider.gameObject.GetInstanceID().Equals(agent.gameObject.GetInstanceID())) {
				closestAgent = agent;
				distance = direction.magnitude;
			}
		}

		if(IPfsm.currentState.GetType() == typeof(IciclePenguinAttackState)) {
			
			//If in the attack state, and no penguins found, change back to the move state.
			if(closestAgent == null) {
				IPfsm.changeState(typeof(IciclePenguinChillinState));
			}
			//If in the attack state, and a penguin was found, update target to that penguin.
			else {
				((IciclePenguinAttackState)IPfsm.currentState).target = (HumanAgent)closestAgent;
			}
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
				Vector2 node = aStar.currPath[i];
				/*node.y*=-1;
				DebugRenderer.drawCircle(DebugRenderer.currentCamera.WorldToScreenPoint(node), 5.0f);
				node.y*=-1;*/
				DebugRenderer.drawCircleWorld(node, 1.0f, Color.red);
			}
			for(int j = 0; j < aStar.prevPath.Count; j++){
				Vector2 node = aStar.prevPath[j];
				DebugRenderer.drawCircleWorld(node, 1.5f, Color.green);
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
		
		Vector2 goal = DebugRenderer.currentCamera.WorldToScreenPoint (aStar.currGoal);
		DebugRenderer.drawCircle (new Vector2 (goal.x, Screen.height - goal.y), 2 * getRadiusCameraSpace ());
	}
}
