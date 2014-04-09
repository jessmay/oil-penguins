using UnityEngine;
using System.Collections;

public class SleepState : State {

	private IciclePenguinFSM IPfsm;
	
	public SleepState (IciclePenguinFSM fsm) : base(fsm) {
		IPfsm = fsm;
	}
	
	public override string getName () {
		return "Sleep State";
	}
	
	public override void enter(){
		//cannot select penguin when sleeping
		IPfsm.penguin.selectable = false;
	}
	
	public override void exit(){
		//Change back to selectable
		IPfsm.penguin.selectable = true;
	}
	
	//check if need to move to new state
	public override void update(){
		//Changes state if needed
		updateState ();
	}
	
	//Sees if there needs to be a state change
	public void updateState(){
		//if sleep timer runs out, wake up and go to Chillin State
		if (IPfsm.penguin.sleepTimer == 0){
			//Change state to chillin
			finiteStateMachine.changeState(typeof(ChillinState));
		}
	}
	
	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}
}
