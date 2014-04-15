using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnBoat : MonoBehaviour {

	public WaveManager waveManager;
	public GameMap gameMap;

	private Quaternion spawnDirection;
	private Vector2 spawnPoint;
	private int numHumans;

	public List<GameObject> humans;

	private bool inPlay;
	private int numHumansSpawned;

	private float fade;

	void Start () {}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(!inPlay)
			return;

		if(waveManager.betweenWaves) {
			transform.position = Mathf.Pow(((waveManager.waveStartTime - Time.time)/WaveManager.timeBetweenWaves), 2) * 100 * WaveManager.timeBetweenWaves * (spawnDirection * Vector2.up) + gameMap.map.cellIndexToWorld(spawnPoint) + new Vector3(0, 0, -1);
		}
		else if(numHumansSpawned == numHumans && humans.Count == 0) {

			if (fade > 0){
				GetComponent<SpriteRenderer>().color = new Color(1,1,1,fade);
				fade -= .01f;
			}
			else {
				--waveManager.activeSpawns;
				inPlay = false;
				gameObject.SetActive(false);
			}
		}

	}

	public void setComponents(GameMap gameMap, WaveManager waveManager) {
		this.gameMap = gameMap;
		this.waveManager = waveManager;
	}

	public void setSpawn(Vector2 spawnPoint, int numHumans) {
		this.spawnPoint = spawnPoint;
		this.numHumans = numHumans;

		transform.rotation = gameMap.map.getSpawnAngle(spawnPoint);
		spawnDirection = transform.rotation * Quaternion.Euler(new Vector3(0,0, 180));

		transform.position = 100 * WaveManager.timeBetweenWaves * (spawnDirection * Vector2.up) + gameMap.map.cellIndexToWorld(spawnPoint) + new Vector3(0, 0, -1);

		//enabled = true;
		inPlay = true;
		numHumansSpawned = 0;
		gameObject.SetActive(true);

		GetComponent<SpriteRenderer>().color = Color.white;
		fade = 1;
	}

	public void spawn() {

		transform.position = gameMap.map.cellIndexToWorld(spawnPoint) + new Vector3(0, 0, -1);
		humans = gameMap.spawnHuman(spawnPoint, numHumans, this);
	}

	public void incrementNumSpawned() {
		++numHumansSpawned;
	}
}
