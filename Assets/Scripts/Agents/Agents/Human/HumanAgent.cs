using UnityEngine;
using System.Collections;

public class HumanAgent : GameAgent, ITarget {

	//Create and initialize human
	public static GameObject CreateAgent(GameObject agent, Vector3 location, Quaternion rotation, GameMap gameMap, Genome genome) {
		
		GameObject newAgent = Agent.CreateAgent(agent, location, rotation, gameMap);
		
		HumanAgent ha = newAgent.GetComponent<HumanAgent>();
		
		ha.brain = genome;

		return newAgent;
	}


	public GameObject Tranquilizer;
	
	private bool holdingICEMachine;

	private static float minDistanceToHolder = 5;

	
	public Genome brain {get; private set;}
	
	protected double[] thoughts;
	protected double[] senses;
	
	public Vector3 startPosition {get; private set;}

	
	public override float getTurnStep() { return turnStep; }
	public override float getMoveStep() { return 3.0f * transform.localScale.x * (holdingICEMachine? 0.6f: 1);}

	protected override float getMaxHealth () { return 100; }

	//Sensor
	public AdjacentAgents adjAgents {get; private set;}

	//FSM


	// Use this for initialization
	protected override void initializeAgent () {
		base.initializeAgent();

		startPosition = transform.position;

		holdingICEMachine = false;

		brain.initialize(this);

		adjAgents = new AdjacentAgents(this, radius * 3, grid);
	}

	// Update is called once per frame
	protected override void updateAgent () {
		base.updateAgent();
		
		//Calculate information for each sensor.
		sense();
		senses = brain.sense(this);
		
		//think about the information collected by the sensors.
		thoughts = brain.think(this, senses);
		
		//act on ANN output.
		brain.act(this, thoughts);
		
		
		//Check for win situation.
		if(holdingICEMachine && map.getCellIndex(transform.position) == map.getCellIndex(startPosition)) {
			
			Debug.Log("Humans Win");
		}
	}

	
	protected override void destroyAgent() {

		//Notify ICEMachine of death if holding the ICEMachine
		if(holdingICEMachine) {
			gameMap.ICEMachineOnMap.GetComponent<ICEMachine>().drop();
		}

		gameMap.HumansOnMap.Remove (gameObject);
	}


	protected override void checkButtons (){
		
		if(Input.GetKeyDown(KeyCode.Space)) {
			Instantiate(Tranquilizer, transform.position + transform.up*(radius + Tranquilizer.renderer.bounds.extents.y), transform.rotation);
		}
	}

	public override void onDeath () {
		Destroy(gameObject);
	}

	private void sense() {
		
		//Get list of nearest agents
		adjAgents.calculate();

	}

	//Get ICE machine's location from map.
	public Vector2 getTarget() {
		
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

	public void pickUp() {
		holdingICEMachine = true;
	}

	public void drop() {
		holdingICEMachine = false;
	}

	protected override void drawStatus() {
		base.drawStatus();
	}

	//Returns when the agent is controllable.
	protected override bool isControllable(){
		return false;
	}
	
	//Draw debug information to the screen.
	protected override void DrawDebugInformation(){

		DebugRenderer.drawCircleWorld(getTarget(),1,Color.green);

		//Draw sensors to the screen.
		{
			//Draw circle for nearest agents
			//adjAgents.drawSensor();
		}

		//Draw debug text to the screen
		{
			//Get agent information
			string debugText = "Agent Id: "+ gameObject.GetInstanceID() +"\n";
			debugText += "Coordinates: " +"("+renderer.bounds.center.x +", "+ renderer.bounds.center.y+")" +"\n";
			debugText += "Heading: " + heading + ".\n\n";
			
			
			debugText += "Senses:\n";
			for (int currSense = 0; senses != null && currSense < senses.Length; ++currSense) {
				debugText += senses[currSense] +"\n";
			}
			debugText += "\n";
			
			
			debugText += "Thoughts:\n";
			for (int currThought = 0; thoughts != null && currThought < thoughts.Length; ++currThought) {
				debugText += thoughts[currThought] +"\n";
			}
			
			debugText += "\n";
			debugText += "Genome: " +brain.GetType().Name +"\n";
			debugText += brain.getDebugInformation() + "\n";
			
			debugText += "Distance from target: " +distanceBetweenPoint(getTarget()) +"\n\n";
			
			
			//Get sensor information
			//debugText += adjAgents.getDebugInformation()+ "\n";
			
			GUI.color = Color.black;
			GUI.Label(new Rect(0, 0, 300, 800), debugText);
		}
	}

}

