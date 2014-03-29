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
		
		createMap(Options.mapName);
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
	
	private void createMap(string mapName) {
		
		Debug.Log("Creating map "+mapName);
		
		if(map != null)
			map.Dispose();

		map = Map.loadMap(mapName, Wall);

		Bounds mapBounds = map.getBounds();
		
		if (grid != null)
			grid.Dispose();
		
		grid = new Grid (map.getMapWidth(), map.getMapHeight(), mapBounds);
		
		transform.localScale = new Vector3(mapBounds.size.x/renderer.bounds.size.x*transform.localScale.x, mapBounds.size.y/renderer.bounds.size.y*transform.localScale.y, transform.localScale.z);
	}

//	public void loadMap (string mapName) {
//		
//		//Reload the same map?
//		//if(map != null && map.name.Equals(mapName))
//		//	return;
//		
//		Debug.Log("Loading Map " +mapName);
//		
//		byte[] bytes = File.ReadAllBytes(Application.dataPath + "/../Maps/"+ mapName +".png");
//		Texture2D mapImage = new Texture2D(1,1);
//		mapImage.LoadImage(bytes);
//		
//		//createMap(mapName, mapImage);//new Map(mapName, Wall, mapImage)
//		
//		//createPlayer();
//	}

	
	//Add type if icicle penguins added.
	public void spawnPenguin(int amount = 1) {

	}


	//Given a human spawn point on the map, spawn the amount of humans. 
	// Humans face towards the center of the map.
	public void spawnHuman(Vector2 location, int amount = 1) {

		for (int currHuman = 0; currHuman < amount; ++currHuman) {

			StartCoroutine(spawnHuman (currHuman * 3.0f, location));
		}
	}

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
