using UnityEngine;
using System.Collections;

public class HumanAgent : TestableAgent {

	[HideInInspector]
	public bool holdingICEMachine;

	public override float getTurnStep() { return turnStep; }
	public override float getMoveStep() { return 3.0f * transform.localScale.x;}
	
	// Use this for initialization
	protected override void initializeAgent () {
		base.initializeAgent();

		holdingICEMachine = false;
	}
	
	
	//Get ICE machine's location from map.
	public override Vector2 getTarget() {

		if(holdingICEMachine)
			return startPosition;
		else
			return gameMap.ICEMachineOnMap.transform.position;
	}
	
	//Nothing to deconstruct
	protected override void destroyAgent() {
		//Notify ICEMachine of death if holding the ICEMachine
		if(holdingICEMachine) {
			gameMap.ICEMachineOnMap.GetComponent<ICEMachine>().drop();
		}
	}
	
	
	// Update is called once per frame
	protected override void updateAgent () {
		base.updateAgent();
	
		if(holdingICEMachine && map.getCellIndex(transform.position) == map.getCellIndex(startPosition))
			Debug.Log("Humans Win");
	}

	public void pickUp() {
		holdingICEMachine = true;
	}

//	void OnGUI() {
//		DebugRenderer.drawCircleWorld(getTarget(),1,Color.green);
//	}


}

