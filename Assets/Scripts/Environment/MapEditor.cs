using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapEditor : MonoBehaviour {

	GameMap gameMap;
	Color currColor;

	Color[] mapColors = {Map.WallColor, Map.HumanSpawnColor, Map.PenguinSpawnColor, Map.ICEMachineColor};
	int colorIndex;

	public Texture2D[] PlaceableItems;

	// Use this for initialization
	void Start () {
		gameMap = GetComponent<GameMap>();
		currColor = Map.WallColor;
		colorIndex = 0;

		visitedCells = new HashSet<Vector2>();
	}

	HashSet<Vector2> visitedCells;

	// Update is called once per frame
	void Update () {

		if(GetComponent<PauseMenu>().isPaused())
			return;


		Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
		pos.z = 0;
		
		Vector2 mapPos = gameMap.map.getCellIndex(pos);
		mapPos -= new Vector2(Mathf.Abs(mapPos.x)%2, Mathf.Abs(mapPos.y)%2);
		mapPos += Vector2.one * 0.5f;

		if(Input.GetMouseButtonUp(0)) {
			visitedCells.Clear();
		}

		//Place/remove item at the given mouse location
		if (Input.GetMouseButton (0) && !(Input.mousePosition.x > Screen.width - PlayGameGUI.GUISize && Input.mousePosition.y < PlayGameGUI.GUISize) && gameMap.map.inBounds(mapPos) && !visitedCells.Contains(mapPos)) {
			//Debug.Log("Click at map position: "+mapPos);

			visitedCells.Add(mapPos);

			if(currColor == Map.WallColor) {

				removeItems(mapPos, colorIndex);

				//If wall already exists at the location, remove it
				if(!gameMap.map.addWall(mapPos))
					gameMap.map.removeWall(mapPos);
			}
			else if(currColor == Map.HumanSpawnColor) { //TODO: Force human spawn points to have space in front.

				if(mapPos.x < 1 || mapPos.x > gameMap.map.getMapWidth()-2 || mapPos.y < 1 || mapPos.y > gameMap.map.getMapHeight() -2) {

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
		if(Input.GetMouseButtonDown(1) && !(Input.mousePosition.x > Screen.width - PlayGameGUI.GUISize && Input.mousePosition.y < PlayGameGUI.GUISize) && gameMap.map.inBounds(mapPos)) {

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

		float xLength = DebugRenderer.worldToCameraLength(gameMap.map.xSize);
		float yLength = DebugRenderer.worldToCameraLength(gameMap.map.ySize);

		//Display each human spawn point
		foreach (Vector2 loc in map.HumanSpawnPoints) {

			Vector3 loc2 = DebugRenderer.currentCamera.WorldToScreenPoint(map.cellIndexToWorld(loc));
			loc2.y = Screen.height - loc2.y;

			//DebugRenderer.drawCircle(loc2, DebugRenderer.worldToCameraLength(1), Map.HumanSpawnColor);

			Rect rect = new Rect(loc2.x - xLength/2, 
			                     loc2.y - yLength/2, 
			                     xLength, 
			                     yLength); 

			GUIUtility.RotateAroundPivot(-map.getSpawnAngle(loc).eulerAngles.z, loc2);

			GUI.DrawTexture(rect, PlaceableItems[1]);

			GUIUtility.RotateAroundPivot(map.getSpawnAngle(loc).eulerAngles.z, loc2);
		}

		//Display penguin spawn point
		if(map.PenguinSpawn != Map.INVALID_LOCATION) {

			Vector2 penLoc = DebugRenderer.currentCamera.WorldToScreenPoint(map.cellIndexToWorld(map.PenguinSpawn));
			penLoc.y = Screen.height - penLoc.y;

			//DebugRenderer.drawCircle(penLoc, DebugRenderer.worldToCameraLength(1), Map.PenguinSpawnColor);

			Rect rect = new Rect(penLoc.x - xLength/2, 
			                     penLoc.y - yLength/2, 
			                     xLength, 
			                     yLength); 
			
			GUI.DrawTexture(rect, PlaceableItems[2]);
		}

		//Display ICE Machine location
		if(map.ICEMachineLocation != Map.INVALID_LOCATION) {

			Vector2 ICELoc = DebugRenderer.currentCamera.WorldToScreenPoint(map.cellIndexToWorld(map.ICEMachineLocation));
			ICELoc.y = Screen.height - ICELoc.y;

			//DebugRenderer.drawCircle(ICELoc, DebugRenderer.worldToCameraLength(1), Map.ICEMachineColor);

			Rect rect = new Rect(ICELoc.x - xLength/2, 
			                     ICELoc.y - yLength/2, 
			                     xLength, 
			                     yLength); 
			
			GUI.DrawTexture(rect, PlaceableItems[3]);
		}


		
		Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
		pos.z = 0;
		
		Vector2 mapPos = gameMap.map.getCellIndex(pos);
		mapPos -= new Vector2(Mathf.Abs(mapPos.x)%2, Mathf.Abs(mapPos.y)%2);
		mapPos += Vector2.one * 0.5f;

		if(map.inBounds(mapPos)) {

			Vector3 cameraCell = DebugRenderer.currentCamera.WorldToScreenPoint(map.cellIndexToWorld(mapPos));
			cameraCell.y = Screen.height - cameraCell.y;

			Rect rect = new Rect(cameraCell.x - xLength/2.0f, 
			                     cameraCell.y - yLength/2.0f, 
			                     xLength, 
			                     yLength); 
			
			Rect rect2 = new Rect(cameraCell.x - xLength, 
			                      cameraCell.y - yLength, 
			                      xLength*2, 
			                      yLength*2);


			//If human spawn, rotate to face inside the map,
			// Draw human sprite,
			// unrotate GUI
			if((colorIndex == 1 && map.getEdgeDirectionVector(mapPos) != Vector2.zero)) {

				DebugRenderer.drawBox(rect2, Color.white);
				//Rotate GUI
				GUIUtility.RotateAroundPivot(-map.getSpawnAngle(mapPos).eulerAngles.z, cameraCell);
				GUI.DrawTexture(colorIndex == 0? rect2: rect, PlaceableItems[colorIndex]);
				GUIUtility.RotateAroundPivot(map.getSpawnAngle(mapPos).eulerAngles.z, cameraCell);
			}

			//Draw sprite at mouse location.
			else if(colorIndex != 1) {

				DebugRenderer.drawBox(rect2, Color.white);

				GUI.DrawTexture(colorIndex == 0? rect2: rect, PlaceableItems[colorIndex]);
			}
		}
	}

}
