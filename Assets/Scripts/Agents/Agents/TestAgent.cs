/*
Joshua Linge
TestAgent.cs

2014-03-17
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TestAgent : Agent {

	private NeuralNet brain;
	private const double FIRED_VALUE = 0.7;
	private double[] thoughts;
	private double[] senses;

	public int numInputs {get; private set;}
	public int numOutputs {get; private set;}
	public int numLayers {get; private set;}
	public int numNeuronsPerLayer {get; private set;}

	public bool userControl;

	public bool testing = true;
	public GeneticAlgorithm geneticAlgorithm;

	//Bonus statistics
	public int numTimesFired {get; private set;}
	public int numTimesRotateLeft {get; private set;}
	public int numTimesRotateRight {get; private set;}
	
	public int numTargetsHit {get; private set;}
	public double targetBonus {get; private set;}
	public int rotBonus {get; private set;}
	public int colBonus {get; private set;}

	//Target information
	public bool targetsEnabled;
	public bool mouseIsTarget;
	private Vector3[] targets;
	public int currTargetIndex = 0;

	//Sensors
	protected Feelers feelers;
	protected AdjacentAgents adjAgents;
	protected PieSlices pieSlices;

	private Vector2 startLocation;
	private Quaternion startRotation;


	// Use this for initialization
	protected override void initializeAgent () {

		numInputs = 4;
	    numOutputs = 2;
	    numLayers = 1;
	    numNeuronsPerLayer = 6;

		targetsEnabled = true;
		mouseIsTarget = false;

		userControl = false;

		//Create sensors
		feelers = new Feelers(this, radius*3); 		//TODO add perpendicular feelers to allow agent to know when it's moving along a wall.
		adjAgents = new AdjacentAgents(this, radius*3, grid);
		pieSlices = new PieSlices(this, adjAgents);

		//Create brain and genetic algorithm
		brain = new NeuralNet(numInputs, numOutputs, numLayers, numNeuronsPerLayer);
		setGeneticAlgorithm( new GeneticAlgorithm(40, numInputs, numOutputs, numLayers, numNeuronsPerLayer));

		startLocation = transform.position;
		startRotation = transform.rotation;

		createFourTargets();

		reset ();
	}


	//Set the agent's genetic algorithm, and set the neural network to the current genome.
	public void setGeneticAlgorithm(GeneticAlgorithm geneticAlgorithm) {

		this.geneticAlgorithm = geneticAlgorithm;	
		this.geneticAlgorithm.resetCurrentGenome();
		brain.replaceWeights(this.geneticAlgorithm.getCurrentGenome().weights);
	}


	//Create nine targets, creating a grid on the map.
	public void createNineTargets() {
		
		targets = new Vector3[9];
		
		int count = 0;
		for (int i = -1; i < 2; ++i)	{
			for (int j = -1; j < 2; ++j)	{
				targets[count] = new Vector3(15*i, 15*j, 0);
				++count;
			}
		}

		currTargetIndex = 0;
		geneticAlgorithm.resetCurrentGenome();
	}


	//Create four targets, each in a different quadrant of the map.
	public void createFourTargets() {
		
		targets = new Vector3[4];
		
		int count = 0;
		for (int i = -1; i < 2; ++i)	{
			for (int j = -1; j < 2; ++j)	{
				if(i == 0 || j == 0)
					continue;
				
				targets[count] = new Vector3(15*i, 15*j, 0);
				++count;
			}
		}

		currTargetIndex = 0;
		geneticAlgorithm.resetCurrentGenome();
	}


	//Nothing to deconstruct
	protected override void destroyAgent() {}


	// Update is called once per frame
	protected override void updateAgent () {

		//Check debug buttons.
		checkButtons();

		//Calculate information for each sensor.
		sense();
		
		//think about the information collected by the sensors.
		think();

		//act if user is not controlling the agent.
		if (!userControl)
			act();

		//If targets are enabled and agent is close to the target,
		// update target bonus and move to the next target.
		if(targetsEnabled && distanceFromTarget() < 1){

			++numTargetsHit;
			targetBonus += numTargetsHit * (GeneticAlgorithm.TICKS_PER_GENOME - geneticAlgorithm.tick)/(double)GeneticAlgorithm.TICKS_PER_GENOME;
			moveToNextTarget();
		}

		//If currently testing, update the genetic algorithm
		if(testing)
			geneticAlgorithm.update(this);

	}


	//Get information about the environment.
	private void sense() {
		
		//Get length of feelers
		feelers.calculate();
		
		//Get list of nearest agents
		adjAgents.calculate();
		
		//Get agents in pie slice angles
		pieSlices.calculate();

		
		//Initialize input based on senses.
		senses = new double[numInputs];
		
		double angle = getAngleToTarget();
		
		senses[0] = targetsEnabled? angle/180: 0;
		
		for (int currFeeler = 0; currFeeler < feelers.numFeelers; ++currFeeler) {
			senses[1+currFeeler] = feelers.feelers[currFeeler].magnitude/ feelers.feelerLength;
		}
	}


	//Think about the information collected.
	private void think() {

		//Give neural network the senses and save the output.
		thoughts = brain.fire(senses);
	}


	//Act on the thoughts made.
	private void act () {

		//Turn right or clockwise
		if(thoughts[0] > FIRED_VALUE) {
			turn(-(float)(thoughts[0]-0.5f)*turnStep);
			++numTimesRotateRight;
		}

		//Turn left or counter clockwise
		else if (thoughts[0] < 1 - FIRED_VALUE) {
			turn((float)(0.5f-thoughts[0])*turnStep);
			++numTimesRotateLeft;
		}

		//Move forward
		if(thoughts[1] > FIRED_VALUE) {
			moveTo((float)thoughts[1] * moveStep);
			++numTimesFired;
		}

		//Move backward
//		else if (thoughts[1] < 1-FIRED_VALUE) {
//			moveTo(-(float)thoughts[1] * moveStep);
//		}


		//TODO Give option to strafe left and right.
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


	//Pause or onpause the learning process.
	public void toggleTesting(float timeScale) {

		testing = !testing;

		//Unpause, resume testing
		if(testing) {

			Debug.Log("Restarting Tests");

			//Reset values
			mouseIsTarget = false;
			userControl = false;
			currTargetIndex = 0;
			Time.timeScale = timeScale;

			reset();

			//Get weights for testing
			brain.replaceWeights(geneticAlgorithm.getCurrentGenome().weights);
			//Reset genetic algorithm.
			geneticAlgorithm.resetCurrentGenome();
		}

		//Pause testing
		else {
			Debug.Log("Stopping testing at index: "+geneticAlgorithm.populationIndex);

			Time.timeScale = 1;
			reset();

			//Set weights to the most fit genome.
			brain.replaceWeights(geneticAlgorithm.mostFit.weights);
		}
	}


	//Get the length of the current list of targets.
	public int totalTargets() {
		return targets.Length;
	}


	//Get the index into the target array.
	public int getCurrentTargetIndex() {
		return currTargetIndex;
	}


	//Get the current target.
	public Vector3 getCurrentTargetVector() {

		//If the mouse cursor location is the target, 
		//return the position of the mouse cursor.
		if(mouseIsTarget) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0;
			return mousePos;
		}
		//Else, get the current target from the target list.
		else {
			return targets[currTargetIndex];
		}
	}


	//Move to the next target.
	//Returns true if cycled through all targets
	public bool moveToNextTarget() {

		currTargetIndex = (currTargetIndex + 1) % targets.Length;

		return currTargetIndex == 0;
	}


	//Return the distance between the agent and the target.
	public double distanceFromTarget () {
		return distanceBetweenPoint(getCurrentTargetVector());
	}


	//Return the angle between this agent's heading vector 
	//and the vector from the agent to the target.
	public double getAngleToTarget() {
		return getAngleToPoint(getCurrentTargetVector());
	}


	//Return the distance moved from the start location.
	public float distanceMoved() {
		return (renderer.bounds.center - (Vector3)startLocation).magnitude;
	}


	//Calculate fitness for the current genome based of the results 
	// of the weights within the neural network.
	public double calculateFitness() {
		
		if(distanceMoved() == 0) {
			return .01;
		}
		
		return    targetBonus 
				+ calcRotBonus()
				+ calcCollBonus()
				+ calcFiredBonus()
				+ calcRotateBothWaysBonus()
				;
	}


	//Calculate the bonus based on how many times the agent rotated.
	private double calcRotBonus() {
		
		if(numTimesRotateLeft + numTimesRotateRight == 0)
			return 0;
		
		return (1 - Math.Max((numTimesRotateLeft + numTimesRotateRight)-GeneticAlgorithm.TICKS_PER_GENOME/2.0, 0.0)/((double)GeneticAlgorithm.TICKS_PER_GENOME/2.0))/2.0;
	}


	//Calculate the bonus based on how many times the agent collided with a wall.
	private double calcCollBonus() {
		return Math.Max((GeneticAlgorithm.TICKS_PER_GENOME - colBonus*2.0)/(double)GeneticAlgorithm.TICKS_PER_GENOME/2.0, 0);
	}


	//Calculate the bonus based on how often the agent moved forward.
	private double calcFiredBonus() {
		return (numTimesFired/(double)GeneticAlgorithm.TICKS_PER_GENOME)/2.0;
	}


	//Calculate the bonus based on rotating in both directions.
	private double calcRotateBothWaysBonus() {
		if(numTimesRotateLeft == 0 || numTimesRotateRight == 0)
			return 0;
		return .5;
	}


	//Replace weights in agent's neural network with the given weights.
	public void replaceBrain(double[][][] weights) {
		brain.replaceWeights(weights);
	}


	//Reset agent to default position and reset statistical values.
	public void reset() {
		
		transform.position = startLocation;
		transform.rotation = startRotation;
		heading = startRotation.z;

		numTimesFired = 0;
		numTargetsHit = 0;

		targetBonus = 0;
		rotBonus = 0;
		colBonus = 0;

		numTimesRotateLeft = 0;
		numTimesRotateRight = 0;
	}


	//Reset or calculate information after the end of a test.
	public void endOfTarget(int target) {

		//If the agent was colliding when the test ended, 
		//calculate the total number of ticks colliding.
		if (collTick != -1){

			colBonus += geneticAlgorithm.tick - collTick;
			collTick = -1;
			collidingWalls.Clear();
		}

		//Set the current target.
		currTargetIndex = target;
	}


	//Reset or calculate information after the end of a generation.
	public void endOfTests() {

		rotBonus = 0;
		colBonus = 0;
	}


	//When the agent collides with a wall, add the wall to a list
	// and save the tick.
	private int collTick = -1;
	private HashSet<int> collidingWalls = new HashSet<int>();
	void OnCollisionEnter2D(Collision2D coll) {

		if (collidingWalls.Count == 0)
			collTick = geneticAlgorithm.tick;

		collidingWalls.Add(coll.gameObject.GetInstanceID());

	}


	//The agent is no longer colliding with a wall, remove it from the list
	// If last wall, update total number of ticks colliding.
	void OnCollisionExit2D(Collision2D coll) {

		collidingWalls.Remove(coll.gameObject.GetInstanceID());

		if(collidingWalls.Count == 0) {
			colBonus += geneticAlgorithm.tick - collTick;
			collTick = -1;
		}
	}


	/*		Debug		*/


	//Returns when the agent is controllable.
	protected override bool isControllable(){
		return userControl;
	}


	//Draw debug information to the screen.
	protected override void DrawDebugInformation(){

		//Draw current target if target is enabled.
		if (targetsEnabled){
			Vector3 t = getCurrentTargetVector();
			Vector3 cst = Camera.main.WorldToScreenPoint(t);
			cst.y = Screen.height - cst.y;

			DebugRenderer.drawCircle(cst, DebugRenderer.worldToCameraLength(1));
		}

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

			debugText += "Targets: " +numTargetsHit +"\n";
			debugText += "Fired: "+numTimesFired +"\n";
			debugText += "rotL:  " +numTimesRotateLeft +"\n";
			debugText += "rotR:  " +numTimesRotateRight +"\n";
			debugText += "rot:  " +calcRotBonus() +"\n";
			debugText += "col:  " +(colBonus + (collTick == -1? 0: geneticAlgorithm.tick - collTick)) +" ("+collidingWalls.Count +")\n";

			debugText += "\n";

			debugText += "Individual["+geneticAlgorithm.populationIndex+"]\n";
			debugText += "CurrentScore["+geneticAlgorithm.currTarget+","+currTargetIndex+"]: " +calculateFitness() + "\n";
			
			debugText += "\n";

			debugText += "Generation: "+ geneticAlgorithm.generation + "\n";
			debugText += "Best Fitness: "+geneticAlgorithm.bestFitness +"\n\n";

			//Get sensor information
			debugText += feelers.getDebugInformation()+ "\n";
			debugText += adjAgents.getDebugInformation()+ "\n";
			debugText += pieSlices.getDebugInformation()+ "\n";
			
			GUI.color = Color.black;
			GUI.Label(new Rect(0, 0, 300, 800), debugText);
		}
	}
}