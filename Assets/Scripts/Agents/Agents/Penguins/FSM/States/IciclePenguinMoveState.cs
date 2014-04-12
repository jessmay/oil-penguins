using UnityEngine;
using System.Collections;

public class IciclePenguinMoveState : State {

	private IciclePenguinFSM IPfsm;
	
	public IciclePenguinMoveState (IciclePenguinFSM fsm) : base(fsm) {
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
	public void updateState(){}
	
	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}

}
