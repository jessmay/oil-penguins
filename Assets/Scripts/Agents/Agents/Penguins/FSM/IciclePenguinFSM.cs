using UnityEngine;
using System.Collections;

public class IciclePenguinFSM : FiniteStateMachine{

	public IciclePenguins penguin;

	public IciclePenguinFSM(IciclePenguins p){
		penguin = p;
	}

	protected override void initialize ()
	{
		addState(new ChillinState(this));
		addState (new SleepState (this));
		addState (new MoveState (this));
		addState (new AttackState (this));
	}

	protected override System.Type getDefaultState ()
	{
		return typeof(ChillinState);
	}

}
