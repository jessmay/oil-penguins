using UnityEngine;
using System.Collections;

public class MapEditor : MonoBehaviour {

	GameMap gameMap;
	Color currColor;

	Color[] mapColors = {Map.WallColor, Map.HumanSpawnColor, Map.PenguinSpawnColor, Map.ICEMachineColor};
	int colorIndex;

	// Use this for initialization
	void Start () {
		gameMap = GetComponent<GameMap>();
		currColor = Map.WallColor;
		colorIndex = 0;
	}


	// Update is called once per frame
	void Update () {

		if(GetComponent<PauseMenu>().isPaused())
			return;


		//Place/remove item at the given mouse location
		if (Input.GetMouseButtonDown (0)) {
			Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;

			Vector2 mapPos = gameMap.map.getCellIndex(pos);
			//Debug.Log("Click at map position: "+mapPos);


			if(currColor == Map.WallColor) {

				removeItems(mapPos, colorIndex);

				//If wall already exists at the location, remove it
				if(!gameMap.map.addWall(mapPos))
					gameMap.map.removeWall(mapPos);
			}
			else if(currColor == Map.HumanSpawnColor) { //TODO: Force human spawn points to have space in front.

				if(mapPos.x == 0 || mapPos.x == gameMap.map.getMapWidth()-1 || mapPos.y == 0 || mapPos.y == gameMap.map.getMapHeight() -1) {

					removeItems(mapPos, colorIndex);

					if(!gameMap.map.HumanSpawnPoints.Contains(mapPos))
						gameMap.map.HumanSpawnPoints.Add (mapPos);
					else
						gameMap.map.HumanSpawnPoints.Remove (mapPos);
				}
				else
					Debug.Log("Human spawn points can only be placed on the edges of the map.");
			}
			else if (currColor == Map.PenguinSpawnColor) {

				removeItems(mapPos, colorIndex);

				if(gameMap.map.PenguinSpawn != mapPos)
					gameMap.map.PenguinSpawn = mapPos;
				else
					gameMap.map.PenguinSpawn = Map.INVALID_LOCATION;
			}
			else if (currColor == Map.ICEMachineColor) {

				removeItems(mapPos, colorIndex);
				
				if(gameMap.map.ICEMachineLocation != mapPos)
					gameMap.map.ICEMachineLocation = mapPos;
				else
					gameMap.map.ICEMachineLocation = Map.INVALID_LOCATION;
			}
		}

		//Right click to remove item from location
		if(Input.GetMouseButtonDown(1)) {
			Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;
			
			Vector2 mapPos = gameMap.map.getCellIndex(pos);

			removeItems(mapPos);

		}

		for (int i = 0; i < 4; ++i) {
			if(Input.GetKeyDown(((i+1)).ToString())) {
				currColor = mapColors[i];
				colorIndex = i;
				//Debug.Log("Color changed to index: "+i);
			}
		}
	}

	private void removeItems(Vector2 mapPos, int index = -1) {

		if(index != 0)
			gameMap.map.removeWall(mapPos);

		if(index != 1)
			gameMap.map.HumanSpawnPoints.Remove (mapPos);

		if(index != 2 && gameMap.map.PenguinSpawn == mapPos)
			gameMap.map.PenguinSpawn = Map.INVALID_LOCATION;

		if(index != 3 && gameMap.map.ICEMachineLocation == mapPos)
			gameMap.map.ICEMachineLocation = Map.INVALID_LOCATION;

	}

	void OnGUI() {

		if(GetComponent<PauseMenu>().isPaused())
			return;

		DebugRenderer.drawBox(0,0, 50,50, 0, Vector2.zero, currColor);

		Map map = gameMap.map;

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

}
