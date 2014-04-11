using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class Explorer1Genome : Genome {
	
	private int currTick;

	//Bonus statistics
	public int numTimesForward {get; private set;}
	public int numTimesBackward {get; private set;}
	public int numTimesRotateLeft {get; private set;}
	public int numTimesRotateRight {get; private set;}
	public int numActingTicks {get; private set;}

	public int numTargetsHit {get; private set;}
	public double targetBonus {get; private set;}
	public int rotBonus {get; private set;}
	public int colBonus {get; private set;}


	private LineOfSight lineOfSight;

	public Explorer1Genome(double[][][] weights) : base(weights) {

		reset();
	}

	public Explorer1Genome() {}

	public override int getNumberOfInputs() { return 7; }
	public override int getNumberOfOutputs() { return 2; }
	public override int getNumberOfLayers() { return 1; }
	public override int getNumberOfNeuronsPerLayer() { return 7; }

	
	public override double getFiredValue() { return 0.7; }
	public override float getLengthOfFeelers(Agent agent) { return agent.getRadius() * feelerLength; }
	public override float getDefaultLengthOfFeelers() { return 30; }
	public override int getNumberOfFeelers(){ return 5; }
	public override int getViewAngle() { return 270; }

	public Feelers feelers {get; private set;}
	
	public override void initialize (Agent agent) {
		
		lineOfSight = new LineOfSight(agent);
		feelers = new Feelers(agent, getLengthOfFeelers(agent), getNumberOfFeelers(), getViewAngle());
	}

	public override void reset() {

		currTick = 0;
		
		numTimesForward = 0;
		numTimesBackward = 0;
		numTimesRotateLeft = 0;
		numTimesRotateRight = 0;
		
		numTargetsHit = 0;
		targetBonus = 0;
		rotBonus = 0;
		colBonus = 0;

		numActingTicks = 0;
	}

	public override void endOfTarget(){

		//If the agent was colliding when the test ended, 
		//calculate the total number of ticks colliding.
		if (collTick != -1){
			
			colBonus += currTick - collTick;
			collTick = -1;
			collidingWalls.Clear();
		}
	}
	
	public override void endOfTests(){
		
	}


	public override double[] sense<A>(A agent) {
		
		feelers.calculate();
		
		//Initialize input based on senses.
		double[] senses = new double[getNumberOfInputs()];

		//Get proper target
		double angle = agent.getAngleToPoint(agent.getTarget());
		
		senses[0] = angle/180;

		lineOfSight.calculate();

		senses[1] = lineOfSight.inSight? 1:0;
		
		for (int currFeeler = 0; currFeeler < feelers.numFeelers; ++currFeeler) {
			senses[2+currFeeler] = feelers.feelers[currFeeler].magnitude/ feelers.feelerLength;
		}

		return senses;
	}


	public override double[] think(Agent agent, double[] senses) {
		return brain.fire(senses);
	}

	public override void act(Agent agent, double[] thoughts) {

		bool act = false;

		//Turn right or clockwise
		if(thoughts[0] > getFiredValue()) {
			agent.turn(-(float)((thoughts[0] - getFiredValue())/(1 - getFiredValue()))*agent.getTurnStep());
			++numTimesRotateRight;

			act = true;
		}
		
		//Turn left or counter clockwise
		else if (thoughts[0] < 1 - getFiredValue()) {
			agent.turn((float)(thoughts[0]/(1 - getFiredValue()))*agent.getTurnStep());
			++numTimesRotateLeft;
			
			act = true;
		}

		//Move forward
		if(thoughts[1] > getFiredValue()) {
			agent.moveTo((float)thoughts[1] * agent.getMoveStep());
			++numTimesForward;

			act = true;
		}
		
		//Move backward
		else if (thoughts[1] < 1-getFiredValue()) {
			agent.moveTo(-(float)thoughts[1] * agent.getMoveStep());
			++numTimesBackward;
			
			act = true;
		}
		
//		//Move forward
//		if(thoughts[1] > getFiredValue()) {
//			agent.moveTo((float)((thoughts[1] - getFiredValue())/(1 - getFiredValue())) * agent.getMoveStep());
//			++numTimesForward;
//			
//			act = true;
//		}
//		
//		//Move backward
//		else if (thoughts[1] < 1-getFiredValue()) {
//			agent.moveTo(-(float)(thoughts[1]/(1 - getFiredValue())) * agent.getMoveStep());
//			++numTimesBackward;
//			
//			act = true;
//		}

		if(act) {
			++numActingTicks;
		}
		
		//TODO Give option to strafe left and right.


	}

	public override void update(TestableAgent agent) {

		++currTick;

		if(agent.distanceBetweenPoint(agent.getTarget()) < .5) {
			++numTargetsHit;
			targetBonus += numTargetsHit * (GeneticAlgorithm.TICKS_PER_GENOME() - currTick)/(double)GeneticAlgorithm.TICKS_PER_GENOME();

			moveToTestStart(agent);
		}
	}


	private int collTick = -1;
	private HashSet<int> collidingWalls = new HashSet<int>();
	public override void OnCollisionEnter(Collision2D collision) {
		
		if (collidingWalls.Count == 0)
			collTick = currTick;
	
		collidingWalls.Add(collision.gameObject.GetInstanceID());
		
	}
	
	
	//The agent is no longer colliding with a wall, remove it from the list
	// If last wall, update total number of ticks colliding.
	public override void OnCollisionExit(Collision2D collision) {
		
		collidingWalls.Remove(collision.gameObject.GetInstanceID());
		
		if(collidingWalls.Count == 0) {
			colBonus += currTick - collTick;
			collTick = -1;
		}
	}


	//Calculate fitness for the current genome based of the results 
	// of the weights within the neural network.
	public override double calculateFitness() {
		
		if(numActingTicks != GeneticAlgorithm.TICKS_PER_GENOME()) {
			return .01;
		}
		
		return    targetBonus 
				+ numTargetsHit
				+ calcRotBonus()
				+ calcMoveBonus()
				+ calcCollBonus()
				;
	}
	
	
	//Calculate the bonus based on how many times the agent rotated.
	private double calcRotBonus() {
		
		if(numTimesRotateLeft == GeneticAlgorithm.TICKS_PER_GENOME() || numTimesRotateRight == GeneticAlgorithm.TICKS_PER_GENOME())
			return 0.1;

		if(numTimesRotateLeft == 0 || numTimesRotateRight == 0)
			return 0.1;

		return Mathf.CeilToInt(numTimesRotateLeft/(float)GeneticAlgorithm.TICKS_PER_GENOME())*0.5 + Mathf.CeilToInt(numTimesRotateRight/(float)GeneticAlgorithm.TICKS_PER_GENOME())*0.5;
	}

	private double calcMoveBonus() {

		if(numTimesForward == GeneticAlgorithm.TICKS_PER_GENOME() || numTimesBackward == GeneticAlgorithm.TICKS_PER_GENOME())
			return 0.25;

		if(numTimesForward == 0 || numTimesForward <= numTimesBackward || numTimesForward <= GeneticAlgorithm.TICKS_PER_GENOME()/2)
			return 0.1;

		return Mathf.CeilToInt(numTimesForward/(float)GeneticAlgorithm.TICKS_PER_GENOME())*0.75 + Mathf.CeilToInt(numTimesBackward/(float)GeneticAlgorithm.TICKS_PER_GENOME())*0.25;
	}
	
	//Calculate the bonus based on how many times the agent collided with a wall.
	private double calcCollBonus() {

		if(colBonus == 0)
			return 1;

		return 0.5 - colBonus/(double)GeneticAlgorithm.TICKS_PER_GENOME() * 0.5;
	}



	public override string getDebugInformation() {

		feelers.drawDebugInformation();
		
		lineOfSight.drawSensor();

		string debugText = "";

		debugText += "Targets: " +numTargetsHit +"\n";
		debugText += "Fired: "+numTimesForward +"\n";
		debugText += "rotL:  " +numTimesRotateLeft +"\n";
		debugText += "rotR:  " +numTimesRotateRight +"\n";
		debugText += "rot:  " +calcRotBonus() +"\n";
		debugText += "col:  " +(colBonus + (collTick == -1? 0: currTick - collTick)) +" ("+collidingWalls.Count +")\n";
		
		return debugText;
	}
}

