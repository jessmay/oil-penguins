using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GameMap : MonoBehaviour{
	
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
	}

	void FixedUpdate() {

		//Spawn a human at a random spawn point.
//		if(GetComponent<MapEditor>() != null && Input.GetMouseButtonDown(1)) {
//			spawnHuman(map.getCellIndex(DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition)), 1);
//			//Vector2 location = map.getRandomHumanSpawn();
//			//spawnHumanImmediate(location, Quaternion.LookRotation(transform.forward, Vector3.zero - map.cellIndexToWorld(location)), new Explorer1Genome());
//		}
	}

	//void OnRenderObject() {
	void OnGUI() {

		if(GUI.Button(new Rect(Screen.width/2 - 100/2, Screen.height - (25 + 50), 100, 50), "Spawn Penguin")) {
			spawnPenguin();
		}

		//if(GetComponent<PauseMenu>().isPaused())
		//	return;

		//Display each human spawn point
//		foreach (Vector2 loc in map.HumanSpawnPoints) {
//
//			Vector3 loc2 = DebugRenderer.currentCamera.WorldToScreenPoint(map.cellIndexToWorld(loc));
//			loc2.y = Screen.height - loc2.y;
//			DebugRenderer.drawCircle(loc2, DebugRenderer.worldToCameraLength(1), Map.HumanSpawnColor);
//		}
//
//		//Display penguin spawn point
//		if(map.PenguinSpawn != Map.INVALID_LOCATION) {
//
//			Vector2 penLoc = DebugRenderer.currentCamera.WorldToScreenPoint(map.cellIndexToWorld(map.PenguinSpawn));
//			penLoc.y = Screen.height - penLoc.y;
//			DebugRenderer.drawCircle(penLoc, DebugRenderer.worldToCameraLength(1), Map.PenguinSpawnColor);
//		}
//
//		//Display ICE Machine location
//		if(map.ICEMachineLocation != Map.INVALID_LOCATION) {
//
//			Vector2 ICELoc = DebugRenderer.currentCamera.WorldToScreenPoint(map.cellIndexToWorld(map.ICEMachineLocation));
//			ICELoc.y = Screen.height - ICELoc.y;
//			DebugRenderer.drawCircle(ICELoc, DebugRenderer.worldToCameraLength(1), Map.ICEMachineColor);
//		}


//		foreach (Vector2 loc in map.HumanSpawnPoints) {
//			
//			DebugRenderer.drawCircleWorld(map.cellIndexToWorld(loc), 1, Map.HumanSpawnColor);
//		}
//		
//		//Display penguin spawn point
//		if(map.PenguinSpawn != Map.INVALID_LOCATION) {
//			
//			DebugRenderer.drawCircleWorld(map.cellIndexToWorld(map.PenguinSpawn), 1, Map.PenguinSpawnColor);
//		}
//		
//		//Display ICE Machine location
//		if(map.ICEMachineLocation != Map.INVALID_LOCATION) {
//			
//			DebugRenderer.drawCircleWorld(map.cellIndexToWorld(map.ICEMachineLocation), 1, Map.ICEMachineColor);
//		}

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
		GameObject penguin = Agent.CreateAgent(Penguin, map.cellIndexToWorld(location), Quaternion.LookRotation(transform.forward, Vector3.zero - map.cellIndexToWorld(location)), this);

		//Initialize penguin specific values
		//penguin.GetComponent<Penguin>();
		
		PenguinsOnMap.Add(penguin);
	}


	public GameObject spawnHumanImmediate(Vector2 location, Quaternion? rotationNullable, Genome genome){

		Quaternion rotation = rotationNullable.HasValue? rotationNullable.Value: Quaternion.LookRotation(transform.forward, Vector3.zero - map.cellIndexToWorld(location));

		//Debug.Log("Spawning new human at "+ map.cellIndexToWorld(location));
		
		GameObject human = TestableAgent.CreateAgent(Human, map.cellIndexToWorld(location), rotation, this, genome);
		
		HumansOnMap.Add(human);

		++totalHumansSpawned;

		return human;
	}

	//Given a human spawn point on the map, spawn the amount of humans. 
	public List<GameObject> spawnHuman(Vector2 location, int amount = 1) {

		List<GameObject> humans = new List<GameObject>(amount);

		for (int currHuman = 0; currHuman < amount; ++currHuman) {

			StartCoroutine(spawnHuman (currHuman * 3.0f, location, humans));
		}

		return humans;
	}

	//Spawns a human at the given location after a delay.
	// Humans face towards the center of the map.
	IEnumerator spawnHuman(float delay, Vector2 location, List<GameObject> humansList) {

		yield return new WaitForSeconds(delay);

		
		string[] files = Directory.GetFiles(Application.dataPath + "/../GA/Genomes/");
		
		//string[] fileList = Directory.GetFiles(path + "/");
		List<string> fileNames = new List<string>();
		
		//Loop through each file in the directory
		for (int currFile = 0; currFile < files.Length; ++currFile) {
			
			if(!files[currFile].Substring(files[currFile].Length-4).Equals(".txt"))
				continue;
	
//			//Add file name to the list of file names
//			//Path and directory information is removed first.
//			fileNames.Add(fileList[currFile].Remove(fileList[currFile].LastIndexOf('.')).Remove(0,fileList[currFile].LastIndexOf('/')+1));
			
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
		//Quaternion.LookRotation(transform.forward, Vector3.zero - map.cellIndexToWorld(location))
		humansList.Add (spawnHumanImmediate(location, map.getSpawnAngle(location), genome));

	}
}
