using UnityEngine;
using System.Collections;

public abstract class TestableAgent : Agent {
	
	public static GameObject CreateAgent(GameObject agent, Vector3 location, Quaternion rotation, Map map, Grid grid, Genome genome) {
		
		GameObject newAgent = Agent.CreateAgent(agent, location, rotation, map, grid);

		TestableAgent ta = newAgent.GetComponent<TestableAgent>();

		ta.brain = genome;
		
		return newAgent;
	}

	public Genome brain {get; private set;}
	
	protected double[] thoughts;
	protected double[] senses;

	//Sensors
	public Feelers feelers {get; private set;}
	public AdjacentAgents adjAgents {get; private set;}
	public PieSlices pieSlices {get; private set;}

	public Vector3 startPosition {get; private set;}
	public Quaternion startRotation {get; private set;}

	public abstract Vector2 getTarget();

	// Use this for initialization
	protected override void initializeAgent () {

		startPosition = transform.position;
		startRotation = transform.rotation;

		//Create sensors
		feelers = new Feelers(this, brain.getLengthOfFeelers(this), brain.getNumberOfFeelers(), brain.getViewAngle());
		//adjAgents = new AdjacentAgents(this, radius*3, grid);
		//pieSlices = new PieSlices(this, adjAgents);

	}
	
	//Replace weights in agent's neural network with the given weights.
	public void replaceBrain(Genome genome) {
		brain = genome;
	}

	// Update is called once per frame
	protected override void updateAgent () {
		
		//Check debug buttons.
		if(debug) 
			checkButtons();
		
		//Calculate information for each sensor.
		sense();
		senses = brain.sense(this);
		
		//think about the information collected by the sensors.
		thoughts = brain.think(this, senses);
		
		//act on ANN output.
		brain.act(this, thoughts);

		if(Options.Testing)
			brain.update(this);
	}
	
	
	//Get information about the environment.
	private void sense() {
		
		//Get length of feelers
		feelers.calculate();
		
		//Get list of nearest agents
		//adjAgents.calculate();
		
		//Get agents in pie slice angles
		//pieSlices.calculate();
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

	void OnCollisionEnter2D(Collision2D collision) {

		if(Options.Testing)
			brain.OnCollisionEnter(collision);
	}
	
	
	void OnCollisionExit2D(Collision2D collision) {

		if(Options.Testing)
			brain.OnCollisionExit(collision);
	}

	public void reset() {

		transform.position = startPosition;
		transform.rotation = startRotation;
		heading = transform.rotation.eulerAngles.z;

		brain.reset();
	}

	public void reset(Vector3 position, Quaternion rotation) {

		transform.position = position;
		transform.rotation = rotation;
		heading = transform.rotation.eulerAngles.z;

		brain.reset();
	}
	
	
	//Returns when the agent is controllable.
	protected override bool isControllable(){
		return false;
	}

	//Draw debug information to the screen.
	protected override void DrawDebugInformation(){
		
		//Draw current target if target is enabled.
		//if (targetsEnabled){
//			Vector3 t = getTarget();
//			Vector3 cst = DebugRenderer.currentCamera.WorldToScreenPoint(t);
//			cst.y = Screen.height - cst.y;
//			
//			DebugRenderer.drawCircle(cst, DebugRenderer.worldToCameraLength(1), Color.green);
		//}
		
		//Draw sensors to the screen.
		{
			// Draw feelers
			feelers.drawSensor();
			
			//Draw circle for nearest agents
			//adjAgents.drawSensor();
			
			//Draw pie slices
			//pieSlices.drawSensor();
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
			debugText += "Genome: " +brain.GetType().Name +"\n";
			debugText += brain.getDebugInformation() + "\n";

			debugText += "Distance from target: " +distanceBetweenPoint(getTarget()) +"\n\n";

			
			//Get sensor information
			debugText += feelers.getDebugInformation()+ "\n";
			//debugText += adjAgents.getDebugInformation()+ "\n";
			//debugText += pieSlices.getDebugInformation()+ "\n";
			
			GUI.color = Color.black;
			GUI.Label(new Rect(0, 0, 300, 800), debugText);
		}
	}
}
