using UnityEngine;
using System.Collections;

public class TrainingAgent : Agent, ITarget {
	
	public static GameObject CreateAgent(GameObject agent, Vector3 location, Quaternion rotation, GameMap gameMap, Genome genome) {
		
		GameObject newAgent = Agent.CreateAgent(agent, location, rotation, gameMap);

		TrainingAgent ta = newAgent.GetComponent<TrainingAgent>();

		ta.brain = genome;
		
		return newAgent;
	}

	public Genome brain {get; private set;}
	
	protected double[] thoughts;
	protected double[] senses;

	public Vector3 startPosition {get; private set;}
	public Quaternion startRotation {get; private set;}

	
	public override float getTurnStep() { return turnStep; }
	public override float getMoveStep() { return 10.0f * transform.localScale.x;}


	// Use this for initialization
	protected override void initializeAgent () {

		startPosition = transform.position;
		startRotation = transform.rotation;

		brain.initialize(this);
	}
	
	//Replace weights in agent's neural network with the given weights.
	public void replaceBrain(Genome genome) {
		brain = genome;
	}

	// Update is called once per frame
	protected override void updateAgent () {
		
		//Calculate information for each sensor.
		senses = brain.sense(this);
		
		//think about the information collected by the sensors.
		thoughts = brain.think(this, senses);
		
		//act on ANN output.
		brain.act(this, thoughts);

		if(Options.Testing)
			brain.update(this);
	}

	protected override void destroyAgent () {}
	
	//Get ICE machine's location from map.
	public Vector2 getTarget() {
		return map.cellIndexToWorld(map.ICEMachineLocation);
	}


	protected override void drawStatus () {}

	protected override void checkButtons () {}

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

			GUI.color = Color.black;
			GUI.Label(new Rect(0, 0, 300, 800), debugText);
		}
	}
}
