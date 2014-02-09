/*
Jessica May
Joshua Linge
Background.cs
 */

using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	public GameObject Wall;
	public GameObject Agent;

	private int mapWidth = 20;
	private int mapHeight = 20;

	private int gridWidth = 10;
	private int gridHeight = 10;


	public Grid grid;
	public Map map;

	// Use this for initialization
	void Start () {

		grid = new Grid (gridWidth, gridHeight, renderer.bounds);
		map = new Map(Wall,mapWidth, mapHeight, renderer.bounds);

	}

	// Update is called once per frame
	void Update () {

		//Place/remove wall at the given mouse location
		if (Input.GetMouseButtonDown (0)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;

			//If wall already exists at the location, remove it
			if(!map.addWall(map.getCellIndex(pos)))
				map.removeWall(map.getCellIndex(pos));
		}

		//Place agent at the given mouse location
		if (Input.GetMouseButtonDown (1)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;
			GameObject a = Instantiate(Agent, pos, Wall.transform.rotation) as GameObject;
			grid.add(a.GetComponent<Agent>());
		}
	}


}
