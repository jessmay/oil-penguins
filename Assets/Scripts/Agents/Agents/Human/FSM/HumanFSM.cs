using UnityEngine;
using System.Collections;

public class HumanFSM : FiniteStateMachine {

	public HumanAgent humanAgent;
	
	public HumanFSM(HumanAgent humanAgent){
		this.humanAgent = humanAgent;
	}

	protected override System.Type getDefaultState () {
		return typeof(HumanMoveState);
	}

	protected override void initialize () {

		addState(new HumanMoveState(this));
		addState(new HumanAttackState(this));
	}
}
