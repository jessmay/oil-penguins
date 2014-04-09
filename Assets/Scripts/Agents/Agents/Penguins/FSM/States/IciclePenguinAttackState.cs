using UnityEngine;
using System.Collections;

public class AttackState : State {

	private IciclePenguinFSM IPfsm;
	
	public AttackState (IciclePenguinFSM fsm) : base(fsm) {
		IPfsm = fsm;
	}
	
	public override string getName () {
		return "Attack State";
	}
	
	public override void enter(){
	}
	
	public override void exit(){
	}
	
	//check if need to move to new state
	public override void update(){
		//attack hoomans

		//Changes state if needed
		updateState ();
	}
	
	//Sees if there needs to be a state change
	public void updateState(){
		//if fully tranquilized, sleep
		if (IPfsm.penguin.health == 0) {
			finiteStateMachine.changeState(typeof(SleepState));
		}

		//if player wants the penguin to move
		if (IPfsm.penguin.hasPath) {
			finiteStateMachine.changeState(typeof(MoveState));
		}

		// if no longer has hooman in its adjacent agent sensors, wait
		if (IPfsm.penguin.adjAgents.near.Count == 0) {
			finiteStateMachine.changeState(typeof(ChillinState));
		}
	}
	
	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}
}
