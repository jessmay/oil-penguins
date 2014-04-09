﻿using UnityEngine;
using System.Collections;

public class MiniMap : MonoBehaviour {

	GameMap gameMap;
	Bounds mapBounds;
	Vector2 size;
	Vector2 onScreenSize;

	float percentOfScreen = 0.10f;

	Vector2 locationOnScreen;

	Color ViewRectColor = Color.magenta;

	public bool display = true;

	// Use this for initialization
	void Start () {
		gameMap = GetComponent<GameMap>();

		mapBounds = gameMap.map.getBounds();

		float max = Mathf.Max(mapBounds.size.x, mapBounds.size.y);

		size = new Vector2(mapBounds.size.x/max, mapBounds.size.y/max);

		max = Mathf.Max(Screen.width, Screen.height);
		onScreenSize = new Vector2((max*percentOfScreen*size.x), (max*percentOfScreen*size.y));
		
		locationOnScreen = new Vector2(Screen.width, onScreenSize.y);
	}

	void Update() {
		
		if(Input.GetKeyDown(KeyCode.M)) {
			display = !display;
		}
	}
	
	// Update is called once per frame
	Vector3 prevMousePos = Vector3.one*-1;
	void FixedUpdate () {

		if(!display)
			return;

		Vector3 mousePos = Input.mousePosition;

		//Move minimap to new location
		if(prevMousePos != Vector3.one*-1) {
			locationOnScreen.x = Mathf.Clamp(locationOnScreen.x +(mousePos - prevMousePos).x, onScreenSize.x, Screen.width);
			locationOnScreen.y = Mathf.Clamp(locationOnScreen.y + (prevMousePos - mousePos).y, onScreenSize.y, Screen.height);
			prevMousePos = Vector3.one*-1;
		}

		//Save mouse location to know how much to move minimap.
		if(Input.GetMouseButton(1) && inBounds(mousePos)) {
			prevMousePos = mousePos;
		}

		//Move camera based on click point.
		if(Input.GetMouseButton(0) && inBounds(mousePos)) {

			Vector2 mapClickLocation = mousePos;
			mapClickLocation.y = Screen.height - mapClickLocation.y;

			mapClickLocation -= locationOnScreen;

			mapClickLocation.x = mapClickLocation.x / onScreenSize.x * mapBounds.size.x + (mapBounds.size.x)/2.0f;
			mapClickLocation.y = -mapClickLocation.y / onScreenSize.y * mapBounds.size.y - (mapBounds.size.y)/2.0f;

			Vector3 cameraPosition = DebugRenderer.currentCamera.transform.position;
			DebugRenderer.currentCamera.GetComponent<CameraController>().moveCameraClamped(cameraPosition, mapClickLocation - (Vector2)cameraPosition);
		}
	}

	private bool inBounds(Vector2 location) {
		return location.x < locationOnScreen.x && location.x > locationOnScreen.x - onScreenSize.x && Screen.height - location.y < locationOnScreen.y && Screen.height - location.y > locationOnScreen.y - onScreenSize.y;
	}

	void OnGUI() {

		if(!display)
			return;

		Vector2 coord = locationOnScreen - onScreenSize;
		//coord.y = Screen.height - coord.y;

		DebugRenderer.drawBox(coord.x, coord.y, onScreenSize.x, onScreenSize.y, 0, Vector3.zero, Color.white);

		float wallSize = onScreenSize.x / gameMap.map.mapWidth;

		//Draw walls
		for (int row = 0; row < gameMap.map.mapWidth; ++row) {
			for(int col = 0; col < gameMap.map.mapHeight; ++col) {
				if(!gameMap.map.canMove[row, col])
					DebugRenderer.drawBox(coord.x + row*wallSize, coord.y+(onScreenSize.y - col*wallSize) - wallSize, wallSize, wallSize, 0, Vector3.zero, Map.WallColor);
			}
		}

		//Draw ICE machine
		{
			Vector3 ICECoord = gameMap.ICEMachineOnMap.transform.position;
			ICECoord = (ICECoord + mapBounds.size/2.0f);
			
			ICECoord.x = ICECoord.x/mapBounds.size.x * onScreenSize.x;
			ICECoord.y = onScreenSize.y - ICECoord.y/mapBounds.size.y * onScreenSize.y;

			Vector2 newCoord = coord + (Vector2)ICECoord;
			
			DebugRenderer.drawBox(new Rect(newCoord.x - wallSize/2, newCoord.y - wallSize/2, wallSize, wallSize), Map.ICEMachineColor);
			//DebugRenderer.drawBox(coord.x + ICECoord.x, ICECoord.y, wallSize, wallSize, 0, Vector3.zero, Map.ICEMachineColor);

		}

		//Draw humans
		foreach(GameObject human in gameMap.HumansOnMap) {

			Vector3 humanCoord = human.transform.position;
			humanCoord = (humanCoord + mapBounds.size/2.0f);

			humanCoord.x = humanCoord.x/mapBounds.size.x * onScreenSize.x;
			humanCoord.y = onScreenSize.y - humanCoord.y/mapBounds.size.y * onScreenSize.y;
	
			Vector2 newCoord = coord + (Vector2)humanCoord;

			DebugRenderer.drawBox(new Rect(newCoord.x - wallSize/2, newCoord.y - wallSize/2, wallSize, wallSize), Map.HumanSpawnColor);
		}

		//Draw penguins
		foreach(GameObject penguin in gameMap.PenguinsOnMap) {
			
			Vector3 penguinCoord = penguin.transform.position;
			penguinCoord = (penguinCoord + mapBounds.size/2.0f);
			
			penguinCoord.x = penguinCoord.x/mapBounds.size.x * onScreenSize.x;
			penguinCoord.y = onScreenSize.y - penguinCoord.y/mapBounds.size.y * onScreenSize.y;
			
			Vector2 newCoord = coord + (Vector2)penguinCoord;

			DebugRenderer.drawBox(new Rect(newCoord.x - wallSize/2, newCoord.y - wallSize/2, wallSize, wallSize), Map.PenguinSpawnColor);
		}

		//Draw view rectangle
		{
			Rect rect = DebugRenderer.currentCamera.rect;
			Vector2 viewPortMax = DebugRenderer.currentCamera.ViewportToWorldPoint(new Vector3(rect.xMax, rect.yMax));
			Vector2 viewPortMin = DebugRenderer.currentCamera.ViewportToWorldPoint(new Vector3(rect.xMin, rect.yMin));

			Vector2 maxB = new Vector2(Mathf.Min(mapBounds.max.x, viewPortMax.x), Mathf.Min(mapBounds.max.y, viewPortMax.y));
			Vector2 minB = new Vector2(Mathf.Max(mapBounds.min.x, viewPortMin.x), Mathf.Max(mapBounds.min.y, viewPortMin.y));

			maxB.x = maxB.x/mapBounds.size.x * onScreenSize.x + (onScreenSize.y)/2.0f;
			maxB.y = maxB.y/mapBounds.size.y * onScreenSize.y - (onScreenSize.y)/2.0f;

			minB.x = minB.x/mapBounds.size.x * onScreenSize.x + (onScreenSize.y)/2.0f;
			minB.y = minB.y/mapBounds.size.y * onScreenSize.y - (onScreenSize.y)/2.0f;

			DebugRenderer.drawLineRect(new Rect(coord.x + minB.x, coord.y - maxB.y, maxB.x - minB.x, maxB.y - minB.y), wallSize/2, ViewRectColor);
		}
	}
}