using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IciclePenguinAttackState : State {

	public HumanAgent target;

	public static float shotCoolDownTime = 3;
	private float nextShotTime = -1;

	public Agent targetAgent;

	private float turnAngle;
	private int turnCount;

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

		if(target != null)
			IPfsm.penguin.seek(target.transform.position);

		turnAngle = IPfsm.penguin.getTurnStep ();
		turnCount = 0;
	}
	
	public override void exit(){}
	
	//check if need to move to new state
	public override void update(){
		//attack hoomans
		double distanceToTarget = IPfsm.penguin.distanceBetweenPoint (target.transform.position);

		if (distanceToTarget > target.getRadius () + IPfsm.penguin.getRadius () * 1.5) {
			IPfsm.penguin.seek (target.transform.position);
			return;
		}
		else{
			if (turnCount < 20 && turnCount > 0) {
				IPfsm.penguin.turn (turnAngle);
				turnCount++;
			}
			else if(turnCount >= 20){
				IPfsm.penguin.turn (-turnAngle);
				turnCount++;
				if(turnCount >= 40){
					turnCount = 0;
				}
			}
		}

		//Find the angle between the human's heading and the target penguin.
		double angleToTarget = IPfsm.penguin.getAngleToPoint(target.transform.position);
		
		//If not facing the target, turn towards it.
		if(angleToTarget != 0 && turnCount == 0) {
			IPfsm.penguin.turn(Mathf.Clamp((float)angleToTarget, -IPfsm.penguin.getTurnStep(), IPfsm.penguin.getTurnStep()));
        }

		//If facing the target and cooldown time has passed, club human
		else if(nextShotTime <= Time.time && turnCount == 0){
			IPfsm.penguin.turn (turnAngle);
			turnCount++;
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
