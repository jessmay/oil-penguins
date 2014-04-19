using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IciclePenguinAttackState : State {

	public HumanAgent target;

	public static float shotCoolDownTime = 3;
	private float nextShotTime = -1;

	public Agent targetAgent;

	private IciclePenguinFSM IPfsm;
	
	public IciclePenguinAttackState (IciclePenguinFSM fsm) : base(fsm) {
		IPfsm = fsm;
	}
	
	public override string getName () {
		return "Attack State";
	}
	
	public override void enter(){
		//update the penguin sprite
		IPfsm.penguin.GetComponent<SpriteRenderer>().sprite = IPfsm.penguin.penguinSprites [1];

		//If first time entering attack state,
		// allow penguin to shoot right away.
		//(Yes, I'm stealing your code)
		//(Does this mean we should make an abstract attack state?)
		if(nextShotTime == -1)
			nextShotTime = Time.time;
	}
	
	public override void exit(){}
	
	//check if need to move to new state
	public override void update(){
		//attack hoomans

		IPfsm.penguin.seek ((Vector2)target.transform.position);

		//Find the angle between the penguin's heading and the target human.
		double angleToTarget = IPfsm.penguin.getAngleToPoint(target.transform.position);
		
		//If not facing the target, turn towards it.
		if(angleToTarget != 0) {
			IPfsm.penguin.turn(Mathf.Clamp((float)angleToTarget, -1.0f, 1.0f));
		}
		//If facing the target and cooldown time has passed, shoot penguin.
		else if(nextShotTime <= Time.time){
			//IPfsm.penguin.club();
			nextShotTime = Time.time + shotCoolDownTime;
		}

		//Changes state if needed
		updateState ();
	}
	
	//Sees if there needs to be a state change
	public void updateState(){
		// if no longer has hooman in its adjacent agent sensors, wait
		if (IPfsm.penguin.adjAgents.near.Count == 0 || target == null) {
			finiteStateMachine.changeState(typeof(IciclePenguinChillinState));
		}
	}
	
	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}
	
}
