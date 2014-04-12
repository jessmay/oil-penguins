using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour {

	public int waveNumber {get; private set;}
	public const int timeBetweenWaves = 30;
	private GameMap gameMap;

	public bool betweenWaves {get; private set;}
	public float waveStartTime {get; private set;}

	private int expectedTotalHumansSpawned;

	//public GameObject Ship;

	// Use this for initialization
	void Start () {
		waveNumber = 1;
		betweenWaves = true;
		gameMap = GetComponent<GameMap>();

		waveStartTime = Time.time + timeBetweenWaves;
	}

	void Update() {
		
		if(Input.GetKeyUp(KeyCode.K)) {

			foreach(GameObject human in gameMap.HumansOnMap) {
				Destroy(human);
			}
			gameMap.HumansOnMap.Clear();
		}
	}

	// Update is called once per frame
	void FixedUpdate () {

		if(betweenWaves && Time.time >= waveStartTime){
			waveStart();
		}
		else if(!betweenWaves && expectedTotalHumansSpawned == gameMap.totalHumansSpawned &&  gameMap.HumansOnMap.Count == 0) {
			waveEnd();
		}
	}

	private static int numHumansOnFirstWave = 3;
	private static int humansPerWave(int waveNumber) {
		return numHumansOnFirstWave + Mathf.RoundToInt(Mathf.Pow(waveNumber-1, 1.5f));
	}



	public void waveStart() {

		waveStartTime = Time.time;

		betweenWaves = false;

		//3 starting humans
		int numHumansToSpawn = humansPerWave(waveNumber);

		expectedTotalHumansSpawned = gameMap.totalHumansSpawned + numHumansToSpawn;

		Debug.Log(numHumansToSpawn +" humans for wave "+waveNumber);

		List<Vector2> allSpawns = new List<Vector2>(gameMap.map.HumanSpawnPoints);

		int numSpawnPoints = Mathf.Min(Mathf.CeilToInt(waveNumber/2.0f), allSpawns.Count);

		Vector2[] waveSpawns = new Vector2[numSpawnPoints];
		int[] humansPerSpawn = new int[numSpawnPoints];

		for(int currSpawn = 0; currSpawn < numSpawnPoints; ++currSpawn) {

			int index = Mathf.FloorToInt(UnityEngine.Random.value*allSpawns.Count);
			waveSpawns[currSpawn] = allSpawns[index];
			allSpawns.RemoveAt(index);

			humansPerSpawn[currSpawn] = numHumansToSpawn/(numSpawnPoints - currSpawn);

			numHumansToSpawn -= humansPerSpawn[currSpawn];

			//List<GameObject> spawnedHumans = 
			gameMap.spawnHuman(waveSpawns[currSpawn], humansPerSpawn[currSpawn]);

			Debug.Log(humansPerSpawn[currSpawn] +" humans at "+ waveSpawns[currSpawn]);

			//Get and use a ship from a pool? Initialize to the number of spawn points on the map?
			//Ship.createShip(waveSpawns[currSpawn], humansPerSpawn[currSpawn]);
			//New spawnpoint(waveSpawns[currSpawn], humansPerSpawn[currSpawn]);
			//spawnPoint.spawn();
			//the spawn method should call gameMap.spawnHuman
		}
	}

	// If wanting to have animation for incoming human waves (Ships arriving at shore)
	// Generate spawn points at end of each wave, and only spawn at the start of the wave.
	private void waveEnd() {
		++waveNumber;
		betweenWaves = true;
		waveStartTime = Time.time + timeBetweenWaves;

		//Notify all penguins to wake up.

		gameMap.sleepingPenguins = 0;
	}
}
