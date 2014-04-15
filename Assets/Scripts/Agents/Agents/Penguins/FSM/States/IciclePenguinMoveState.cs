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
	
	public override void enter(){
		//updates the penguins sprite
		IPfsm.penguin.GetComponent<SpriteRenderer>().sprite = IPfsm.penguin.penguinSprites [2];

		IPfsm.penguin.aStar.findTarget = true;
	}
	
	public override void exit(){}
	
	//check if need to move to new state
	public override void update(){
		//move along TODO
		//updates A*
		IPfsm.penguin.aStar.aStarUpdate ();

		//Changes state if needed
		updateState ();
	}
	
	//Sees if there needs to be a state change
	public void updateState(){
		if (!IPfsm.penguin.aStar.findTarget) {
			finiteStateMachine.changeState(typeof(IciclePenguinChillinState));
		}

	}
	
	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}

}
