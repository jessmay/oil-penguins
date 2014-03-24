/*
Jessica May
Joshua Linge
Player.cs

Updated by Joshua Linge on 2014-03-17
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Agent {

	private bool seekMouse;

	//Sensors
	protected Feelers feelers;
	protected AdjacentAgents adjAgents;
	protected PieSlices pieSlices;


	// Use this for initialization
	protected override void initializeAgent () {

		seekMouse = false;

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

		if(seekMouse)
			seek(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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

		if(Input.GetKeyDown(KeyCode.S)) {
			seekMouse = !seekMouse;
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
	}
}