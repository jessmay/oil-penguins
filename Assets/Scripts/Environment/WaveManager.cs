using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour {

	public GameObject SpawnBoat;

	public int waveNumber {get; private set;}
	public const int timeBetweenWaves = 30;
	private GameMap gameMap;

	public bool betweenWaves {get; private set;}
	public float waveStartTime {get; private set;}
	public float previousWaveFinishTime {get; private set;}

	private int expectedTotalHumansSpawned;
	
	private GameObject[] spawnBoats;
	public int activeSpawns;

	// Use this for initialization
	void Start () {

		gameMap = GetComponent<GameMap>();

		spawnBoats = new GameObject[gameMap.map.HumanSpawnPoints.Count];
		waveNumber = 0;

		waveEnd();
	}

	void Update() {
		
//		if(Input.GetKeyUp(KeyCode.K)) {
//
//			foreach(GameObject human in gameMap.HumansOnMap) {
//				human.GetComponent<HumanAgent>().onDeath();
//			}
//		}
	}

	// Update is called once per frame
	void FixedUpdate () {

		if(betweenWaves && Time.time >= waveStartTime){
			waveStart();
		}
		else if(!betweenWaves && expectedTotalHumansSpawned == gameMap.totalHumansSpawned && gameMap.HumansOnMap.Count == 0 && activeSpawns == 0) {
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

		for (int currSpawn = 0; currSpawn < gameMap.map.HumanSpawnPoints.Count && spawnBoats[currSpawn] != null; ++currSpawn) {
			spawnBoats[currSpawn].GetComponent<SpawnBoat>().spawn();
		}

	}

	// If wanting to have animation for incoming human waves (Ships arriving at shore)
	// Generate spawn points at end of each wave, and only spawn at the start of the wave.
	private void waveEnd() {
		++waveNumber;
		betweenWaves = true;
		waveStartTime = Time.time + timeBetweenWaves;

		previousWaveFinishTime = Time.time;

		++gameMap.numPenguinsSpawnable;

		//Notify all penguins to wake up.
		//gameMap.sleepingPenguins = 0;




		
		int numHumansToSpawn = humansPerWave(waveNumber);
		
		expectedTotalHumansSpawned = gameMap.totalHumansSpawned + numHumansToSpawn;
		
		Debug.Log(numHumansToSpawn +" humans for wave "+waveNumber);
		
		List<Vector2> allSpawns = new List<Vector2>(gameMap.map.HumanSpawnPoints);
		
		int numSpawnPoints = Mathf.Min(Mathf.CeilToInt(waveNumber/2.0f), allSpawns.Count);
		
		Vector2[] waveSpawns = new Vector2[numSpawnPoints];
		int[] humansPerSpawn = new int[numSpawnPoints];

		activeSpawns = numSpawnPoints;

		for(int currSpawn = 0; currSpawn < numSpawnPoints; ++currSpawn) {
			
			int index = Mathf.FloorToInt(UnityEngine.Random.value*allSpawns.Count);
			waveSpawns[currSpawn] = allSpawns[index];
			allSpawns.RemoveAt(index);
			
			humansPerSpawn[currSpawn] = numHumansToSpawn/(numSpawnPoints - currSpawn);
			
			numHumansToSpawn -= humansPerSpawn[currSpawn];
			
			//Debug.Log(humansPerSpawn[currSpawn] +" humans at "+ waveSpawns[currSpawn]);

			if(spawnBoats[currSpawn] == null) {
				spawnBoats[currSpawn] = Instantiate(SpawnBoat, -1 * Vector3.forward, gameMap.map.getSpawnAngle(waveSpawns[currSpawn])) as GameObject;
				spawnBoats[currSpawn].GetComponent<SpawnBoat>().setComponents(gameMap, this);
			}

			spawnBoats[currSpawn].GetComponent<SpawnBoat>().setSpawn(waveSpawns[currSpawn], humansPerSpawn[currSpawn]);
		}
	}
}
