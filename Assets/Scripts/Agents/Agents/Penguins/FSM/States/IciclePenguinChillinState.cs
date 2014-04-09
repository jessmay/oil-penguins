using UnityEngine;
using System.Collections;

public class ChillinState : State {

	private IciclePenguinFSM IPfsm;

	public ChillinState (IciclePenguinFSM fsm) : base(fsm) {
		IPfsm = fsm;
	}

	public override string getName () {
		return "Chillin State";
	}

	public override void enter(){}

	public override void exit(){}

	//check if need to move to new state
	public override void update(){
		//Changes state if needed
		updateState ();
	}

	//Sees if there needs to be a state change
	public void updateState(){
		//if fully tranquilized, fall asleep
		if (IPfsm.penguin.health == 0) {
			//move to sleep state
			finiteStateMachine.changeState(typeof(SleepState));
		}
		
		// if has path, transition to move state
		if (IPfsm.penguin.hasPath) {
			//move to move state
			//finiteStateMachine.changeState(typeof(MoveState));
		}
		
		// if hooman in adjAgentSensors, attack
		if(IPfsm.penguin.adjAgents.near.Count > 0){
			//move to attack state
			//finiteStateMachine.changeState(typeof(AttackState));
		}
	}

	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}
}
