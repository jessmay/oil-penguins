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
	}

	protected override System.Type getDefaultState ()
	{
		return typeof(ChillinState);
	}

}
