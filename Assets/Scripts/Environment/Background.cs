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

	private int mapWidth = 25;
	private int mapHeight = 15;

	private int gridWidth = 10;
	private int gridHeight = 10;

	public Grid grid;
	public Map map;

	// Use this for initialization
	void Start () {

		createDefaultMap();
	}

	private void createDefaultMap() {

		//Debug.Log("Creating Default Map");

		if(map != null)
			map.Dispose();
		
		map = new Map("Default", Wall, mapWidth, mapHeight);
		
		Bounds mapBounds = map.getBounds();

		if (grid != null)
			grid.Dispose();

		grid = new Grid (gridWidth, gridHeight, mapBounds);
		
		transform.localScale = new Vector3(mapBounds.size.x/renderer.bounds.size.x*transform.localScale.x, mapBounds.size.y/renderer.bounds.size.y*transform.localScale.y, transform.localScale.z);
	}

	// Update is called once per frame
	// Main game loop
	void Update () {

		//Place/remove wall at the given mouse location
		if (!Input.GetKey (KeyCode.LeftShift) && Input.GetMouseButtonDown (0)) {
			Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;

			//If wall already exists at the location, remove it
			if(!map.addWall(map.getCellIndex(pos)))
				map.removeWall(map.getCellIndex(pos));
		}

		//Place agent at the given mouse location
		if (!Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown (1)) {
			Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;
			GameObject a = Instantiate(Agent, pos, Wall.transform.rotation) as GameObject;
			Agent agent = a.GetComponent<Agent>();
			agent.grid = grid;
			agent.map = map;
			grid.add(agent);
		}

//		if(Input.GetKeyDown(KeyCode.Alpha1)) {
//			createDefaultMap();
//		}
	}

	void OnGUI() {
//
//		Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
//		pos.z = 0;
//
//		string debugText = pos.ToString() +"\n" + map.getCellIndex(pos) + "\n";
//
//		
//		GUI.color = Color.black;
//		GUI.Label(new Rect(0, 0, 300, 800), debugText);
//		
//		for(int i = 0; i < map.getMapWidth(); i++){
//			for(int j = 0; j < map.getMapHeight(); j++){
//
//				Vector2 node = map.cellIndexToWorld(new Vector2(i, j));
//				node = DebugRenderer.currentCamera.WorldToScreenPoint(node);
//				node.y = Screen.height - node.y;
//
//				if(!map.canMove[i, j]){
//					DebugRenderer.drawCircle(node, DebugRenderer.worldToCameraLength(1));
//				}
//			}
//		}
	}
	
}
