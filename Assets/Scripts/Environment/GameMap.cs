using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GameMap : MonoBehaviour{

	public PauseMenu pauseMenu;
	
	public GameObject Wall;
	public GameObject Human;
	public GameObject Penguin;
	public GameObject ICEMachine;

	public Grid grid {get; private set;}
	public Map map {get; private set;}


	public int totalHumansSpawned {get; private set;}
	public List<GameObject> HumansOnMap {get; private set;}
	public List<GameObject> PenguinsOnMap {get; private set;}
	public GameObject ICEMachineOnMap {get; private set;}

	[HideInInspector]
	public int humansKilled;
	[HideInInspector]
	public int sleepingPenguins;

	public float gameStartTime {get; private set;}

	void Awake() {

		HumansOnMap = new List<GameObject>();
		PenguinsOnMap = new List<GameObject>();

		if (Options.mapName != null)
			loadMap(Options.mapName);
		else {
			Options.mapName = "New Map";
			createMap(Options.mapName, Options.mapSize);
		}
		Options.gameMap = this;

		gameStartTime = Time.time;
	}

	void FixedUpdate() {

	}


	private void createMap(string mapName, Vector2 mapSize) {
		
		Debug.Log("Creating map "+mapName +" with a size of "+mapSize+".");
		
		if(map != null)
			map.Dispose();
		
		map = new Map(mapName, Wall, (int)mapSize.x, (int)mapSize.y);
		
		initialize();
	}

	//Load the given map name and dispose of all previous used resouces.
	private void loadMap(string mapName) {
		
		Debug.Log("Loading map "+mapName);
		
		if(map != null)
			map.Dispose();

		map = Map.loadMap(mapName, Wall);

		initialize();
	}


	private void initialize() {
		
		Bounds mapBounds = map.getBounds();
		
		if (grid != null)
			grid.Dispose();
		
		grid = new Grid (map.getMapWidth(), map.getMapHeight(), mapBounds);
		
		transform.localScale = new Vector3(mapBounds.size.x/renderer.bounds.size.x*transform.localScale.x, mapBounds.size.y/renderer.bounds.size.y*transform.localScale.y, transform.localScale.z);

		HumansOnMap.Clear();
		PenguinsOnMap.Clear();
		
		totalHumansSpawned = 0;

		if(ICEMachine != null)
			ICEMachineOnMap = Instantiate(ICEMachine, map.cellIndexToWorld(map.ICEMachineLocation), ICEMachine.transform.rotation) as GameObject;
	}

	
	//Force to only spawn one penguin so that no penguin spawns on top of another?
	//	Should be a check in the manager script.

	//Add type if icicle penguins added.
	public void spawnPenguin(int amount = 1) {
		
		for (int currPenguin = 0; currPenguin < amount; ++currPenguin) {
			
			StartCoroutine(spawnPenguin (currPenguin * 3.0f, map.PenguinSpawn));
		}
	}

	//Spawns a penguin at the given location after a delay.
	// Penguins face towards the center of the map.
	IEnumerator spawnPenguin(float delay, Vector2 location) {
		
		yield return new WaitForSeconds(delay);
		
		Debug.Log("Spawning new penguin at "+ map.cellIndexToWorld(location));
		GameObject penguin = Agent.CreateAgent(Penguin, map.cellIndexToWorld(location) + new Vector3(Random.value*2-1, Random.value*2-1,0), Quaternion.LookRotation(transform.forward, Vector3.zero - map.cellIndexToWorld(location)), this);

		//Initialize penguin specific values
		//penguin.GetComponent<Penguin>();
		
		PenguinsOnMap.Add(penguin);
	}


	public GameObject spawnHumanImmediate(Vector2 location, Quaternion? rotationNullable, Genome genome){

		Quaternion rotation = rotationNullable.HasValue? rotationNullable.Value: Quaternion.LookRotation(transform.forward, Vector3.zero - map.cellIndexToWorld(location));

		GameObject human;

		if(Options.Testing) {
			human = TrainingAgent.CreateAgent(Human, map.cellIndexToWorld(location), rotation, this, genome);
		}
		else {
			human = HumanAgent.CreateAgent(Human, map.cellIndexToWorld(location), rotation, this, genome);
		}
		
		HumansOnMap.Add(human);

		++totalHumansSpawned;

		return human;
	}

	//Given a human spawn point on the map, spawn the amount of humans. 
	public List<GameObject> spawnHuman(Vector2 location, int amount, SpawnBoat spawnBoat) {
		
		List<GameObject> humans = new List<GameObject>(amount);
		
		for (int currHuman = 0; currHuman < amount; ++currHuman) {
			
			StartCoroutine(spawnHuman (currHuman * 3.0f, location, humans, spawnBoat));
		}

		return humans;
	}

	//Spawns a human at the given location after a delay.
	// Humans face towards the center of the map.
	IEnumerator spawnHuman(float delay, Vector2 location, List<GameObject> humansList, SpawnBoat spawnBoat = null) {

		yield return new WaitForSeconds(delay);

		
		string[] files = Directory.GetFiles(Application.dataPath + "/../GA/Genomes/");
		
		List<string> fileNames = new List<string>();
		
		//Loop through each file in the directory
		for (int currFile = 0; currFile < files.Length; ++currFile) {
			
			if(!files[currFile].Substring(files[currFile].Length-4).Equals(".txt"))
				continue;
	
			fileNames.Add(files[currFile]);
		}
		
		files = fileNames.ToArray();
		
		Genome genome;
		if(files.Length != 0){
			
			int index = Random.Range(0, files.Length);
			genome = Genome.load(File.ReadAllText(files[index]));
			//Debug.Log("Genome from file: "+ files[index]);
		}
		else {
			genome = new FiveFeelerGenome();
		}


		GameObject human = spawnHumanImmediate(location, map.getSpawnAngle(location), genome);

		//Quaternion.LookRotation(transform.forward, Vector3.zero - map.cellIndexToWorld(location))
		humansList.Add (human);

		if(spawnBoat != null){
			human.GetComponent<HumanAgent>().spawnBoat = spawnBoat;
			spawnBoat.incrementNumSpawned();
		}
	}
}
