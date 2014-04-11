using UnityEngine;
using System.Collections;

public class HumanAgent : TestableAgent {

	public GameObject Tranquilizer;

	private bool holdingICEMachine;

	private static float minDistanceToHolder = 5;

	public override float getTurnStep() { return turnStep; }
	public override float getMoveStep() { return 3.0f * transform.localScale.x * (holdingICEMachine? 0.6f: 1);}
	
	// Use this for initialization
	protected override void initializeAgent () {
		base.initializeAgent();

		holdingICEMachine = false;
	}

	void Update() {

		if(Input.GetKeyDown(KeyCode.Space)) {
			Instantiate(Tranquilizer, transform.position + transform.up*(radius + Tranquilizer.renderer.bounds.extents.y), transform.rotation);
		}
	}
	
	
	//Get ICE machine's location from map.
	public override Vector2 getTarget() {

		//Holding the ICE Machine, return to beach/spawn.
		if(holdingICEMachine)
			return startPosition;

		else {

			ICEMachine ice = gameMap.ICEMachineOnMap.GetComponent<ICEMachine>();

			//No one is holding the ICE Machine, head to its location.
			if(!ice.held)
				return gameMap.ICEMachineOnMap.transform.position;

			//Someone else is holding the ICE Machine.
			else {
				Agent agent = ice.holder.GetComponent<Agent>();

				//Too close to holder, move away from holder.
				if(agent.distanceBetweenPoint(transform.position) < (minDistanceToHolder + 1)) {
					return ice.holder.transform.position + (transform.position - ice.holder.transform.position).normalized * (minDistanceToHolder + 3);
				}

				float angleBetween = (float)agent.getAngleToPoint(transform.position);

				//Not directly infront of the holder of the ICE Machine, move to surround.
				if(Mathf.Abs(angleBetween) > 30){
					return ice.holder.transform.position + (transform.position - ice.holder.transform.position).normalized * minDistanceToHolder;
				}

				//Directly in front of the holder, move to the side.
				else {

					return ice.holder.transform.position + Quaternion.Euler(0, 0, Mathf.Sign(angleBetween) * 30) * (transform.position - ice.holder.transform.position).normalized * 5;
				}
			}
		}
	}
	
	protected override void destroyAgent() {

		//Notify ICEMachine of death if holding the ICEMachine
		if(holdingICEMachine) {
			gameMap.ICEMachineOnMap.GetComponent<ICEMachine>().drop();
		}
	}
	
	
	// Update is called once per frame
	protected override void updateAgent () {
		base.updateAgent();
	
		if(holdingICEMachine && map.getCellIndex(transform.position) == map.getCellIndex(startPosition)) {
		
			Debug.Log("Humans Win");
		}
	}

	public void pickUp() {
		holdingICEMachine = true;
	}

	public void drop() {
		holdingICEMachine = false;
	}

//	void OnGUI() {
//		DebugRenderer.drawCircleWorld(getTarget(),1,Color.green);
//	}


}

