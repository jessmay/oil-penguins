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
	void FixedUpdate () {

		//Place/remove wall at the given mouse location
		if (Input.GetMouseButtonDown (0)) {
			Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;

			Vector2 mapPos = gameMap.map.getCellIndex(pos);



			if(currColor == Map.WallColor) {

				removeItems(mapPos, colorIndex);

				//If wall already exists at the location, remove it
				if(!gameMap.map.addWall(mapPos))
					gameMap.map.removeWall(mapPos);
			}
			else if(currColor == Map.HumanSpawnColor) {

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

		for (int i = 0; i < 4; ++i) {
			if(Input.GetKeyDown(((i+1)).ToString())) {
				currColor = mapColors[i];
				colorIndex = i;
			}
		}

		if(Input.GetKeyDown(KeyCode.F2)) {
			gameMap.map.saveMap(Options.mapName +"_New");
		}
	}

	private void removeItems(Vector2 mapPos, int index) {

		if(colorIndex != 0)
			gameMap.map.removeWall(mapPos);

		if(colorIndex != 1)
			gameMap.map.HumanSpawnPoints.Remove (mapPos);

		if(colorIndex != 2 && gameMap.map.PenguinSpawn == mapPos)
			gameMap.map.PenguinSpawn = Map.INVALID_LOCATION;

		if(colorIndex != 3 && gameMap.map.ICEMachineLocation == mapPos)
			gameMap.map.ICEMachineLocation = Map.INVALID_LOCATION;

	}

	void OnGUI() {
		DebugRenderer.drawBox(0,0, 50,50, 0, Vector2.zero, currColor);
	}

}
