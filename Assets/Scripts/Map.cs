/*
Jessica May
Joshua Linge
Map.cs
 */

using UnityEngine;
using System.Collections;

public class Map {

	private GameObject wall;

	private int mapWidth = 20;
	private int mapHeight = 20;
	
	private float xSize;
	private float ySize;
	
	private Bounds bounds;
	private Vector3 center;

	private GameObject[,] board;

	public Map(GameObject w, int width, int height, Bounds b) {

		wall = w;
		
		mapWidth = width;
		mapHeight = height;

		bounds = b;
		center = bounds.center;

		//The size the wall needs to be to fit mapWidth x mapHeight within the bounds
		xSize = bounds.size.x/mapWidth;
		ySize = bounds.size.y/mapHeight;

		//The scale value required to change the walls to the required width and height.
		float wallX = xSize/(wall.renderer.bounds.size.x/wall.transform.localScale.x);
		float wallY = ySize/(wall.renderer.bounds.size.y/wall.transform.localScale.y);

		//Scale wall to fit
		wall.transform.localScale = new Vector3(wallX, wallY, 1);
		
		//Board to hold all walls
		board = new GameObject[mapHeight,mapWidth];
		
		createBorder();
	}

	//Put walls along the edges of the map
	private void createBorder() {

		//Place boundry walls on the top and bottom sides.
		for (int i = 0; i < mapWidth; ++i) {
			
			placeWall(new Vector2(i, 0));
			placeWall(new Vector2(i, mapHeight-1));
		}

		//Place boundry walls on the left and right sides.
		for (int j = 1; j < mapHeight-1; ++j) {
			
			placeWall(new Vector2(0, j));
			placeWall(new Vector2(mapWidth-1, j));
		}
	}

	//Check to see if the cell index is within the bounds of the map
	public bool inBounds (Vector2 coord) {
		return coord.x >= 0 && coord.x < mapWidth && coord.y >= 0 && coord.y < mapHeight;
	}

	//Given a world coordinate, translate it into an index into the board.
	public Vector2 getCellIndex(Vector2 coord){
		Vector2 cellIndex = new Vector2 ();

		cellIndex.x = (int)((coord.x - center.x) / xSize + mapWidth/2);
		cellIndex.y = (int)((coord.y - center.y) / ySize + mapHeight/2);

		return cellIndex;
	}

	//Given a cell index, translate into a world coordinate centered at that cell.
	public Vector3 cellIndexToWorld (Vector2 coord) {
		Vector3 worldPoint = new Vector3(0,0, center.z);

		worldPoint.x = center.x+xSize/2+(coord.x-mapWidth/2)*xSize;
		worldPoint.y = center.y+ySize/2+(coord.y-mapHeight/2)*ySize;

		return worldPoint;
	}

	//Add a wall at the given world coordinate.
	//Returns true if the wall was added.
	//Returns false if location is out of bounds or a wall has already been placed.
	public bool addWall(Vector3 coord) {
		return addWall(getCellIndex(coord));
	}

	//Add a wall at the given cell index.
	//Returns true if the wall was added.
	//Returns false if location is out of bounds or a wall has already been placed.
	public bool addWall(Vector2 coord) {

		//Location out of bounds or already has a wall placed.
		if(!inBounds(coord) || board[(int)coord.y,(int)coord.x] != null)
			return false;

		placeWall(coord);
		return true;
	}

	//Create and place a wall at the given cell index
	private void placeWall(Vector2 coord) {
		Vector3 placeLocation = cellIndexToWorld(coord);
		GameObject w = GameObject.Instantiate (wall, placeLocation, wall.transform.rotation) as GameObject;
		board[(int)coord.y,(int)coord.x] = w;
	}

	//Remove a wall at the given world coordinate.
	//Returns true if the wall was removed.
	//Returns false if the location is out of bonds or there is no wall at the location.
	public bool removeWall(Vector3 coord) {
		return removeWall(getCellIndex(coord));
	}

	//Remove a wall at the given cell index
	//Returns true if the wall was removed.
	//Returns false if the location is out of bonds or there is no wall at the location.
	public bool removeWall(Vector2 coord) {

		//Location out of bounds or does not contain a wall
		if (!inBounds(coord) || board[(int)coord.y,(int)coord.x] == null) {
			return false;
		}
		GameObject w = board[(int)coord.y,(int)coord.x];
		GameObject.Destroy(w);

		board[(int)coord.y,(int)coord.x] = null;
		return true;
	}
}
