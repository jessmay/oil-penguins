using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class BasicGenome : Genome {


	private int currTick;

	//Bonus statistics
	public int numTimesFired {get; private set;}
	public int numTimesRotateLeft {get; private set;}
	public int numTimesRotateRight {get; private set;}
	
	public int numTargetsHit {get; private set;}
	public double targetBonus {get; private set;}
	public int rotBonus {get; private set;}
	public int colBonus {get; private set;}

	public BasicGenome(double[][][] weights) : base(weights) {

		currTick = 0;

		numTimesFired = 0;
		numTimesRotateLeft = 0;
		numTimesRotateRight = 0;

		numTargetsHit = 0;
		targetBonus = 0;
		rotBonus = 0;
		colBonus = 0;
	}

	
	public override double getFiredValue() { return 0.7; }
	public override int getNumberOfFeelers(){ return 3; }

	public override double[] sense(TestableAgent agent) {
		//Initialize input based on senses.
		double[] senses = new double[4];

		//Get proper target
		double angle = agent.getAngleToPoint(agent.getTarget());
		
		senses[0] = angle/180;
		
		for (int currFeeler = 0; currFeeler < agent.feelers.numFeelers; ++currFeeler) {
			senses[1+currFeeler] = agent.feelers.feelers[currFeeler].magnitude/ agent.feelers.feelerLength;
		}

		return senses;
	}


	public override double[] think(TestableAgent agent, double[] senses) {
		return brain.fire(senses);
	}

	public override void act(TestableAgent agent, double[] thoughts) {
		
		//Turn right or clockwise
		if(thoughts[0] > getFiredValue()) {
			agent.turn(-(float)(thoughts[0]-0.5f)*agent.getTurnStep());
			++numTimesRotateRight;
		}
		
		//Turn left or counter clockwise
		else if (thoughts[0] < 1 - getFiredValue()) {
			agent.turn((float)(0.5f-thoughts[0])*agent.getTurnStep());
			++numTimesRotateLeft;
		}
		
		//Move forward
		if(thoughts[1] > getFiredValue()) {
			agent.moveTo((float)thoughts[1] * agent.getMoveStep());
			++numTimesFired;
		}
		
		//Move backward
//		else if (thoughts[1] < 1-getFiredValue()) {
//			agent.moveTo(-(float)thoughts[1] * agent.getMoveStep());
//		}
		
		
		//TODO Give option to strafe left and right.


	}

	public override void update(TestableAgent agent) {

		++currTick;


//		if(targetsEnabled && distanceFromTarget() < 1){
//			
//			++numTargetsHit;
//			targetBonus += numTargetsHit * (GeneticAlgorithm.TICKS_PER_GENOME - geneticAlgorithm.tick)/(double)GeneticAlgorithm.TICKS_PER_GENOME;
//			moveToNextTarget();
//		}
	}


	private int collTick = -1;
	private HashSet<int> collidingWalls = new HashSet<int>();
	public override void OnCollisionEnter(Collision2D collision) {
		
		if (collidingWalls.Count == 0)
			collTick = currTick;//geneticAlgorithm.tick;
	
		collidingWalls.Add(collision.gameObject.GetInstanceID());
		
	}
	
	
	//The agent is no longer colliding with a wall, remove it from the list
	// If last wall, update total number of ticks colliding.
	public override void OnCollisionExit(Collision2D collision) {
		
		collidingWalls.Remove(collision.gameObject.GetInstanceID());
		
		if(collidingWalls.Count == 0) {
			colBonus += currTick - collTick;	//geneticAlgorithm.tick - collTick;
			collTick = -1;
		}
	}


	//Calculate fitness for the current genome based of the results 
	// of the weights within the neural network.
	public override double calculateFitness() {
		
//		if(distanceMoved() == 0) {
//			return .01;
//		}
		
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


	public override string getDebugInformation() {
		string debugText = "";

		debugText += "Targets: " +numTargetsHit +"\n";
		debugText += "Fired: "+numTimesFired +"\n";
		debugText += "rotL:  " +numTimesRotateLeft +"\n";
		debugText += "rotR:  " +numTimesRotateRight +"\n";
		debugText += "rot:  " +calcRotBonus() +"\n";
		//debugText += "col:  " +(colBonus + (collTick == -1? 0: geneticAlgorithm.tick - collTick)) +" ("+collidingWalls.Count +")\n";
		
		return debugText;
	}
}
