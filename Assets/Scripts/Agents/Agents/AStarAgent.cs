/*
Jessica May
Joshua Linge
Player.cs
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarAgent : Agent {

	//Sensors
	protected Feelers feelers;
	protected AdjacentAgents adjAgents;
	protected PieSlices pieSlices;


	// Use this for initialization
	protected override void initializeAgent () {

		//Sensors
		feelers = new Feelers(this, radius*3);
		adjAgents = new AdjacentAgents(this, radius*3, grid);
		pieSlices = new PieSlices(this, adjAgents);

	}

	
	//Nothing to deconstruct
	protected override void destroyAgent() {}
	
	
	// Update is called once per frame
	protected override void updateAgent () {
		
		//Calculate information for each sensor.
		sense();
		
		//think
		//Thinking handled by the user.
		
		//act
		checkButtons();
		



	}
	
	
	//Get information about the environment.
	private void sense() {
		
		//Get length of feelers
		feelers.calculate();
		
		//Get list of nearest agents
		adjAgents.calculate();
		
		//Get agents in pie slice angles
		pieSlices.calculate();
	}
	
	
	//Check for debug button presses
	private void checkButtons () {
		
		//display feelers
		if (Input.GetKeyDown(KeyCode.Z)) {
			feelers.toggleDisplay();
		}
		
		//display adjacent agents
		if(Input.GetKeyDown(KeyCode.C)){
			adjAgents.toggleDisplay();
		}
		
		//display pie slices
		if(Input.GetKeyDown(KeyCode.X)){
			pieSlices.toggleDisplay();
		}

		
		
		//Assignment 2
		// If the source node or target node change, the aStar path needs to be updated
		bool sourceChange = false;
		bool targetChange = false;
		
		//Assignment 2
		//Designate source location; shift + left click
		if (Input.GetKey (KeyCode.LeftShift) && Input.GetMouseButtonDown (0)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			//if new source location, sets values of source to mouse position
			if((int)pos.x != source.x || (int)pos.y != source.y){
				source = pos;
				sourceCell = grid.getCellIndex(source);
				sourceChange = true;
			}
		}
		
		//Assignment 2
		//Designate target location; left control+right click
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1)){
			Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			//if new target location, sets values of target to mouse position
			if((int)pos.x != target.x || (int)pos.y != target.y){
				target = pos;
				targetCell = grid.getCellIndex(target);
				targetChange = true;
			}
			
		}
		
		//Assignment 2
		//if there has been a change in the findTarget value during this call of update
		bool findChange = false;
		//tells the player whether or not to perform A* and seek the target
		if(Input.GetKeyDown(KeyCode.F2)){
			findTarget = !findTarget;
			findChange = true;
		}
		
		// Assignment 2
		// tells the player whether or not to display the source node for debugging purposes
		if (Input.GetKeyDown (KeyCode.U)) {
			drawSource = !drawSource;
		}
		
		//Assignment 2
		// tells the player whether or not to display the target node for debugging purposes
		if (Input.GetKeyDown (KeyCode.I)) {
			drawTarget = !drawTarget;
		}
		
		//Assignment 2
		// tells the player whether or not to display the nav graph for debugging purposes
		if (Input.GetKeyDown (KeyCode.O)) {
			drawNodes = !drawNodes;
		}
		
		//Assignment 2
		// tells the player whether or not to display the A* path for debugging purposes
		if (Input.GetKeyDown (KeyCode.P)) {
			drawPath = !drawPath;
		}





		
		
		//Assignment 2
		//Get list of close nodes, gets the closest, seeks, and calculates astar path
		if((findChange || targetChange || sourceChange) && findTarget && source != target){
			//Gets list of close nodes within one cell of the agent
			closeNodes = new List<Vector2>();
			closeNodes = map.getNearNodes(this);
			
			// Gets the closest node from the list of close nodes and sets it as the source node and the currGoal
			currGoal = new Vector2();
			currGoal = closestUnobstructed();
			source = new Vector2();
			source = map.cellIndexToWorld(currGoal);
			
			//gets aStar path if there is one, otherwise turns target seeking off
			if(!aStar(source)){
				findTarget = false;
			}
			
			pathIndex = 0;
		}
		
		//Assignment 2
		//Seeks along A* path if findTarget is true
		if(findTarget){
			
			//If the player is at the target, no more need to find the target
			if(map.getCellIndex(renderer.bounds.center) == map.getCellIndex(target)){
				pathIndex = 0;
				findTarget = false;
			}
			//Otherwise seek towards the current goal location in the aStar path
			else {
				
				//If in the cell index of current goal and not on target, close enough, check next location to seek
				if (Vector2.Distance(map.getCellIndex(renderer.bounds.center), currGoal) <= .1 && pathIndex < currPath.Count-1)
				{
					pathIndex++;
					currGoal = new Vector2();
					currGoal = (Vector2)currPath[pathIndex];
					seek (map.cellIndexToWorld(currGoal));
				}
				//If not in the cell index of current goal, keep seeking to that current goal
				else if (Vector2.Distance(map.getCellIndex(renderer.bounds.center), currGoal) > .1)
				{
					seek(map.cellIndexToWorld(currGoal));
				}
			}
		}
	}
	
	
	/*		Debug		*/
	
	
	//Returns when the agent is controllable.
	protected override bool isControllable(){
		return true;
	}
	
	//Draw debug information to the screen.
	protected override void DrawDebugInformation(){
		
		//Draw sensors to the screen.
		{
			// Draw feelers
			feelers.drawSensor();
			
			//Draw circle for nearest agents
			adjAgents.drawSensor();
			
			//Draw pie slices
			pieSlices.drawSensor();
		}
		
		//Draw debug text to the screen
		{
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

		Vector2 goal = Camera.main.WorldToScreenPoint (map.cellIndexToWorld(currGoal));
		DebugRenderer.drawCircle (new Vector2 (goal.x, Screen.height - goal.y), 2 * getRadiusCameraSpace ());
	}
}