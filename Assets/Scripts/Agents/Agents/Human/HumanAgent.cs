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

	public Sprite[] humanSprites;

	public GameObject Tranquilizer;
	
	private bool holdingICEMachine;

	private static float minDistanceToHolder = 5;

	
	public Genome brain {get; private set;}
	
	public double[] thoughts;
	public double[] senses;
	
	public Vector3 startPosition {get; private set;}

	
	public override float getTurnStep() { return turnStep; }
	public override float getMoveStep() { return 6.0f * transform.localScale.x * (holdingICEMachine? 0.6f: 1);}

	protected override float getMaxHealth () { return 100; }

	//Sensor
	public AdjacentAgents adjAgents {get; private set;}

	//FSM
	public HumanFSM humanFSM {get; private set;}

	// Use this for initialization
	protected override void initializeAgent () {
		base.initializeAgent();

		startPosition = transform.position;

		holdingICEMachine = false;

		brain.initialize(this);

		adjAgents = new AdjacentAgents(this, radius * 8, grid, typeof(IciclePenguins));
		adjAgents.toggleDisplay();

		GetComponent<SpriteRenderer>().sprite = humanSprites[Random.Range(0, humanSprites.Length)];

		humanFSM = new HumanFSM(this);
	}

	// Update is called once per frame
	protected override void updateAgent () {
		base.updateAgent();

		//Calculate information for each sensor.
		sense();

		//Check for change of state if not holding the ICE Machine.
		if(!holdingICEMachine)
			checkForStateChange();

		//Update finite state machine
		humanFSM.update();
		
		//Check for win situation.
		if(holdingICEMachine && map.getCellIndex(transform.position) == map.getCellIndex(startPosition)) {

			gameMap.pauseMenu.pause(typeof(PlayGamePauseStateGameOver));
			//Debug.Log("Humans Win");
		}
	}


	private void checkForStateChange() {

		//Find closest unobstructed penguin
		Agent closestAgent = null;
		float distance = float.MaxValue;

		//Loop though all agents within range
		foreach(Agent agent in adjAgents.near) {

			//Calculate the vector between this human and the penguin.
			Vector2 direction = agent.transform.position - transform.position;

			//if the penguin is not in the sleep state or is not closer than what has already been found, ignore.
			if(((IciclePenguins)agent).IPfsm.currentState.GetType() == typeof(IciclePenguinSleepState) || distance < direction.magnitude)
				continue;

			//Raycast to see if there is line of sight to the penguin.
			RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + (direction.normalized * radius * 1.01f), direction.normalized, direction.magnitude);

			//If the object found is the same as the penguin we are considering, set as the current closest agent.
			if(hit.collider.gameObject.GetInstanceID().Equals(agent.gameObject.GetInstanceID())) {
				closestAgent = agent;
				distance = direction.magnitude;
			}
		}

		if(humanFSM.currentState.GetType() == typeof(HumanAttackState)) {

			//If in the attack state, and no penguins found, change back to the move state.
			if(closestAgent == null) {
				humanFSM.changeState(typeof(HumanMoveState));
			}
			//If in the attack state, and a penguin was found, update target to that penguin.
			else {
				((HumanAttackState)humanFSM.currentState).targetAgent = closestAgent;
			}
		}

		//If in the move state and there is a penguin within range, change to the attack state with it as the target.
		else if(humanFSM.currentState.GetType() == typeof(HumanMoveState) && closestAgent != null) {

			humanFSM.changeState(typeof(HumanAttackState));
			((HumanAttackState)humanFSM.currentState).targetAgent = closestAgent;
		}
	}

	protected override void destroyAgent() {

		//If holding the ICEMachine, notify of death.
		if(holdingICEMachine) {
			gameMap.ICEMachineOnMap.GetComponent<ICEMachine>().drop();
		}

		gameMap.HumansOnMap.Remove (gameObject);
		++gameMap.humansKilled;
	}

	
	public void shoot() {
		Instantiate(Tranquilizer, transform.position + transform.up*(radius + Tranquilizer.renderer.bounds.extents.y), transform.rotation);
	}

	protected override void checkButtons (){
		
//		if(Input.GetKeyDown(KeyCode.Space)) {
//			shoot();
//		}
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

		//In case human was pushed into the ICE Machine.
		if(humanFSM.currentState.GetType() != typeof(HumanMoveState))
			humanFSM.changeState(typeof(HumanMoveState));
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
			adjAgents.drawSensor();
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
			debugText += adjAgents.getDebugInformation()+ "\n";
			
			GUI.color = Color.black;
			GUI.Label(new Rect(0, 0, 300, 800), debugText);
		}
	}

}

