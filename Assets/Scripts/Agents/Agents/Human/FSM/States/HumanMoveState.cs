using UnityEngine;
using System.Collections;

public class HumanMoveState : HumanState {
	
	public HumanMoveState (HumanFSM humanFSM) : base(humanFSM) {}
	
	public override string getName () {
		return "Human Move State";
	}
	
	public override void enter(){}
	
	public override void exit(){}
	
	public override void update(){

		//Collect information from the sensors.
		humanFSM.humanAgent.senses = humanFSM.humanAgent.brain.sense(humanFSM.humanAgent);
		
		//think about the information collected by the sensors.
		humanFSM.humanAgent.thoughts = humanFSM.humanAgent.brain.think(humanFSM.humanAgent, humanFSM.humanAgent.senses);
		
		//act on ANN output.
		humanFSM.humanAgent.brain.act(humanFSM.humanAgent, humanFSM.humanAgent.thoughts);

	}
	
	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}
}
