using UnityEngine;
using System.Collections;

public class IciclePenguinChillinState : State {

	private IciclePenguinFSM IPfsm;

	public IciclePenguinChillinState (IciclePenguinFSM fsm) : base(fsm) {
		IPfsm = fsm;
	}

	public override string getName () {
		return "Chillin State";
	}

	public override void enter(){
		//update the penguin sprite
		if(IPfsm.penguin != null)
			IPfsm.penguin.GetComponent<SpriteRenderer>().sprite = IPfsm.penguin.penguinSprites [0];
	}

	public override void exit(){}

	//check if need to move to new state
	public override void update(){
		//Changes state if needed
		updateState ();
	}

	//Sees if there needs to be a state change
	public void updateState(){
		// if hooman in adjAgentSensors, attack
		if(IPfsm.penguin.adjAgents.near.Count > 0){
			//move to attack state
			finiteStateMachine.changeState(typeof(IciclePenguinAttackState));
		}
	}

	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}
}
