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

	public List<GameObject> HumansOnMap;
	public List<GameObject> PenguinsOnMap;

	void Start() {

		HumansOnMap = new List<GameObject>();
		PenguinsOnMap = new List<GameObject>();

		if (Options.mapName != null)
			loadMap(Options.mapName);
		else
			createMap(Options.mapName, Options.mapSize);
		Options.gameMap = this;
	}

	void FixedUpdate() {

		//Spawn a human at a random spawn point.
		if(GetComponent<MapEditor>() == null && Input.GetMouseButtonDown(1)) {
			spawnHuman(map.HumanSpawnPoints[Mathf.FloorToInt(Random.value*map.HumanSpawnPoints.Count)], 1);
		}
	}

	void OnGUI() {


//		Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
//		pos.z = 0;
//
//		string debugText = pos.ToString() +"\n" + map.getCellIndex(pos) + "\n";
//
//		//GUI.color = Color.black;
//		GUI.Label(new Rect(0, 0, 300, 800), debugText);


		//Display each human spawn point
		foreach (Vector2 loc in map.HumanSpawnPoints) {

			Vector3 loc2 = DebugRenderer.currentCamera.WorldToScreenPoint(map.cellIndexToWorld(loc));
			loc2.y = Screen.height - loc2.y;
			DebugRenderer.drawCircle(loc2, DebugRenderer.worldToCameraLength(1), Map.HumanSpawnColor);
		}

		//Display penguin spawn point
		if(map.PenguinSpawn != Map.INVALID_LOCATION) {

			Vector2 penLoc = DebugRenderer.currentCamera.WorldToScreenPoint(map.cellIndexToWorld(map.PenguinSpawn));
			penLoc.y = Screen.height - penLoc.y;
			DebugRenderer.drawCircle(penLoc, DebugRenderer.worldToCameraLength(1), Map.PenguinSpawnColor);
		}

		//Display ICE Machine location
		if(map.ICEMachineLocation != Map.INVALID_LOCATION) {

			Vector2 ICELoc = DebugRenderer.currentCamera.WorldToScreenPoint(map.cellIndexToWorld(map.ICEMachineLocation));
			ICELoc.y = Screen.height - ICELoc.y;
			DebugRenderer.drawCircle(ICELoc, DebugRenderer.worldToCameraLength(1), Map.ICEMachineColor);
		}

	}


	private void createMap(string mapName, Vector2 mapSize) {
		
		Debug.Log("Creating map "+mapName);
		
		if(map != null)
			map.Dispose();
		
		map = new Map(mapName, Wall, (int)mapSize.x, (int)mapSize.y);
		
		Bounds mapBounds = map.getBounds();
		
		if (grid != null)
			grid.Dispose();
		
		grid = new Grid (map.getMapWidth(), map.getMapHeight(), mapBounds);
		
		transform.localScale = new Vector3(mapBounds.size.x/renderer.bounds.size.x*transform.localScale.x, mapBounds.size.y/renderer.bounds.size.y*transform.localScale.y, transform.localScale.z);
		
		HumansOnMap.Clear();
		PenguinsOnMap.Clear();
	}

	//Load the given map name and dispose of all previous used resouces.
	private void loadMap(string mapName) {
		
		Debug.Log("Loading map "+mapName);
		
		if(map != null)
			map.Dispose();

		map = Map.loadMap(mapName, Wall);

		Bounds mapBounds = map.getBounds();
		
		if (grid != null)
			grid.Dispose();
		
		grid = new Grid (map.getMapWidth(), map.getMapHeight(), mapBounds);
		
		transform.localScale = new Vector3(mapBounds.size.x/renderer.bounds.size.x*transform.localScale.x, mapBounds.size.y/renderer.bounds.size.y*transform.localScale.y, transform.localScale.z);
	
		HumansOnMap.Clear();
		PenguinsOnMap.Clear();
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
		GameObject penguin = Agent.CreateAgent(Penguin, map.cellIndexToWorld(location), Quaternion.LookRotation(transform.forward, Vector3.zero - map.cellIndexToWorld(location)), map, grid);

		//Initialize penguin specific values
		//penguin.GetComponent<Penguin>();
		
		PenguinsOnMap.Add(penguin);
	}


	//Given a human spawn point on the map, spawn the amount of humans. 
	public void spawnHuman(Vector2 location, int amount = 1) {

		for (int currHuman = 0; currHuman < amount; ++currHuman) {

			StartCoroutine(spawnHuman (currHuman * 3.0f, location));
		}
	}

	//Spawns a human at the given location after a delay.
	// Humans face towards the center of the map.
	IEnumerator spawnHuman(float delay, Vector2 location) {

		yield return new WaitForSeconds(delay);

		Debug.Log("Spawning new human at "+ map.cellIndexToWorld(location));
		GameObject human = Agent.CreateAgent(Human, map.cellIndexToWorld(location), Quaternion.LookRotation(transform.forward, Vector3.zero - map.cellIndexToWorld(location)), map, grid);

		string[] files = Directory.GetFiles(Application.dataPath + "/../GA/Genomes/");

		int index = Random.Range(2, files.Length);
		Genome genome = BasicGenome.load(File.ReadAllText(files[index]));
		
		human.GetComponent<TestableAgent>().replaceBrain(genome);

		HumansOnMap.Add(human);
	}
}
