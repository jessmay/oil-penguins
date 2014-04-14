using UnityEngine;
using System;
using System.Collections;

public class HumanAttackState : HumanState {

	public static float shotCoolDownTime = 5;
	private float nextShotTime = -1;

	public Agent targetAgent;

	public HumanAttackState (HumanFSM humanFSM) : base(humanFSM) {}
	
	public override string getName () {
		return "Human Attack State";
	}
	
	public override void enter(){

		//If first time entering attack state,
		// allow human to shoot right away.
		if(nextShotTime == -1)
			nextShotTime = Time.time;
	}
	
	public override void exit(){
		humanFSM.humanAgent.hit = false;
	}
	
	public override void update(){

		//Find the angle between the human's heading and the target penguin.
		double angleToTarget = humanFSM.humanAgent.getAngleToPoint(targetAgent.transform.position);

		//If not facing the target, turn towards it.
		if(angleToTarget != 0) {
			humanFSM.humanAgent.turn(Mathf.Clamp((float)angleToTarget, -1.0f, 1.0f));
		}
		//If facing the target and cooldown time has passed, shoot penguin.
		else if(nextShotTime <= Time.time){
			humanFSM.humanAgent.shoot();
			nextShotTime = Time.time + shotCoolDownTime;
		}
	}

	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}
}
