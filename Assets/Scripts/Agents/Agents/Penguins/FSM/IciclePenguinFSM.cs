using UnityEngine;
using System.Collections;

public class IciclePenguinFSM : FiniteStateMachine{

	public IciclePenguins penguin;

	public IciclePenguinFSM(IciclePenguins p){
		penguin = p;
	}

	protected override void initialize ()
	{
		addState(new IciclePenguinChillinState(this));
		addState (new IciclePenguinSleepState (this));
		addState (new IciclePenguinMoveState (this));
		addState (new IciclePenguinAttackState (this));
	}

	protected override System.Type getDefaultState ()
	{
		return typeof(IciclePenguinChillinState);
	}

}
