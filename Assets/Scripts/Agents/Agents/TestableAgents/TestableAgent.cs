using UnityEngine;
using System.Collections;

public abstract class TestableAgent : Agent {
	
	private Genome brain;
	
	private double[] thoughts;
	private double[] senses;

	//Sensors
	public Feelers feelers {get; private set;}
	public AdjacentAgents adjAgents {get; private set;}
	public PieSlices pieSlices {get; private set;}


	public abstract int getNumberOfFeelers();
	public abstract Vector2 getTarget();

	// Use this for initialization
	protected override void initializeAgent () {
		
		//Create sensors
		feelers = new Feelers(this, radius*3, getNumberOfFeelers());
		adjAgents = new AdjacentAgents(this, radius*3, grid);
		pieSlices = new PieSlices(this, adjAgents);

	}
	
	//Replace weights in agent's neural network with the given weights.
	public void replaceBrain(Genome genome) {
		brain = genome;
	}
	
	//Nothing to deconstruct
	protected override void destroyAgent() {}
	
	
	// Update is called once per frame
	protected override void updateAgent () {
		
		//Check debug buttons.
		checkButtons();
		
		//Calculate information for each sensor.
		sense();
		senses = brain.sense(this);
		
		//think about the information collected by the sensors.
		//think();
		thoughts = brain.think(this, senses);
		
		//act on ANN output.
		//act();
		brain.act(this, thoughts);
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
	}
	
	
	//Returns when the agent is controllable.
	protected override bool isControllable(){
		return false;
	}
	
	
	//Draw debug information to the screen.
	protected override void DrawDebugInformation(){
		
		//Draw current target if target is enabled.
		//if (targetsEnabled){
			Vector3 t = getTarget();
			Vector3 cst = DebugRenderer.currentCamera.WorldToScreenPoint(t);
			cst.y = Screen.height - cst.y;
			
			DebugRenderer.drawCircle(cst, DebugRenderer.worldToCameraLength(1));
		//}
		
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
			
			
			debugText += "Senses:\n";
			for (int currSense = 0; senses != null && currSense < senses.Length; ++currSense) {
				debugText += senses[currSense] +"\n";
			}
			debugText += "\n";
			
			
			debugText += "Thoughts:\n";
			for (int currThought = 0; thoughts != null && currThought < thoughts.Length; ++currThought) {
				debugText += thoughts[currThought] +"\n";
			}
			
			debugText += "\n";
			
			debugText += brain.getDebugInformation() + "\n";
			
			//Get sensor information
			debugText += feelers.getDebugInformation()+ "\n";
			debugText += adjAgents.getDebugInformation()+ "\n";
			debugText += pieSlices.getDebugInformation()+ "\n";
			
			GUI.color = Color.black;
			GUI.Label(new Rect(0, 0, 300, 800), debugText);
		}
	}
}
