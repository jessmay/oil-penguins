using UnityEngine;
using System.Collections;

public abstract class HumanState : State {

	protected HumanFSM humanFSM;

	public HumanState (HumanFSM humanFSM) : base(humanFSM) {

		this.humanFSM = humanFSM;
	}
}

