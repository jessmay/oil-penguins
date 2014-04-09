﻿using UnityEngine;
using System.Collections;

public class MoveState : State {

	private IciclePenguinFSM IPfsm;
	
	public MoveState (IciclePenguinFSM fsm) : base(fsm) {
		IPfsm = fsm;
	}
	
	public override string getName () {
		return "Move State";
	}
	
	public override void enter(){}
	
	public override void exit(){}
	
	//check if need to move to new state
	public override void update(){
		//move along TODO

		//Changes state if needed
		updateState ();
	}
	
	//Sees if there needs to be a state change
	public void updateState(){

		//if health equals 0, then sleep
		if (IPfsm.penguin.health == 0) {
			//change state to sleep
			finiteStateMachine.changeState(typeof(SleepState));
		}

		//if at destination change to wait state
		if (!IPfsm.penguin.hasPath) {
			//change state to wait
			finiteStateMachine.changeState(typeof(ChillinState));
		}
	}
	
	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}

}