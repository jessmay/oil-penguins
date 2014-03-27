/*
Jessica May
Joshua Linge
Map.cs

Updated by Joshua Linge on 2014-03-17
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Map : IDisposable {

	public string name;

	private GameObject wall;

	private int mapWidth;
	private int mapHeight;
	
	private float xSize;
	private float ySize;
	
	//private Bounds bounds;
	private Vector3 center;

	private GameObject[,] board;
	public bool[,] canMove;

	public Map(string n, GameObject w, int width, int height) {

		init(n, w, width, height);

		createBorder();
	}

	public Map(string n, GameObject w, Texture2D map) {

		init (n, w, map.width, map.height);

		readMap(map);
	}
			
	private void init (string n, GameObject w, int width, int height) {

		name = n;

		wall = w;

		createBoard(width, height);

		center = Vector3.zero;
		xSize = wall.renderer.bounds.size.x;
		ySize = wall.renderer.bounds.size.y;


		//bounds = b;
		//center = bounds.center;
		
		//The size the wall needs to be to fit mapWidth x mapHeight within the bounds
		//xSize = bounds.size.x/mapWidth;
		//ySize = bounds.size.y/mapHeight;

		//The scale value required to change the walls to the required width and height.
		//float wallX = xSize/(wall.renderer.bounds.size.x/wall.transform.localScale.x);
		//float wallY = ySize/(wall.renderer.bounds.size.y/wall.transform.localScale.y);
		
		//Scale wall to fit
		//wall.transform.localScale = new Vector3(wallX, wallY, 1);

	}

	public Bounds getBounds() {
		return new Bounds(center, new Vector3(mapWidth*xSize, mapHeight * ySize, 0));
	}

	private void createBoard(int width, int height) {

		mapWidth = width;
		mapHeight = height;

		//Board to hold all walls
		board = new GameObject[mapHeight,mapWidth];

		//contains whether or not a cell can be moved into based on walls
		canMove = new bool[mapWidth, mapHeight];
		for (int i = 0; i < mapWidth; i++) {
			for(int j = 0; j < mapHeight; j++){
				canMove[i,j] = true;
			}
		}
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

	//Place walls based off given map texture
	private void readMap(Texture2D map) {

		removeAllWalls();

		createBoard(map.width, map.height);

		for (int x = 0; x < map.width; ++x) {
			for (int y = 0; y < map.height; ++y) {

				if (map.GetPixel(x,y) == Color.black) {
					addWall(new Vector2(x, y));
				}
				else {
					removeWall(new Vector2(x,y));
				}
			}
		}

	}

	public Texture2D saveMap () {

		Texture2D map = new Texture2D(mapWidth, mapHeight);

		for (int x = 0; x < map.width; ++x) {
			for (int y = 0; y < map.height; ++y) {
				
				if (board[y,x] != null) {
					map.SetPixel(x,y, Color.black);
				}
				else {
					map.SetPixel(x,y, Color.clear);
				}
			}
		}

		map.Apply();

		return map;
	}

	//Check to see if the cell index is within the bounds of the map
	public bool inBounds (Vector2 coord) {
		return coord.x >= 0 && coord.x < mapWidth && coord.y >= 0 && coord.y < mapHeight;
	}

	//Given a world coordinate, translate it into an index into the board.
	public Vector2 getCellIndex(Vector2 coord){
		Vector2 cellIndex = new Vector2 ();

		cellIndex.x = Mathf.FloorToInt((float)((coord.x - center.x + (mapWidth%2 == 1? xSize/2.0:0)) / xSize + mapWidth/2));
		cellIndex.y = Mathf.FloorToInt((float)((coord.y - center.y + (mapHeight%2 == 1? ySize/2.0:0)) / ySize + mapHeight/2));

		return cellIndex;
	}

	//Given a cell index, translate into a world coordinate centered at that cell.
	public Vector3 cellIndexToWorld (Vector2 coord) {

		Vector3 worldPoint = new Vector3(0,0, center.z);

		worldPoint.x = center.x+xSize/2+(coord.x-mapWidth/2)*xSize - (mapWidth%2 == 1? xSize/2:0);
		worldPoint.y = center.y+ySize/2+(coord.y-mapHeight/2)*ySize - (mapHeight%2 == 1? ySize/2:0);

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
		canMove [(int)coord.x, (int)coord.y] = false;
        //Debug.Log((int)coord.y + " " + (int)coord.x);
	}

	//Remove all walls from the map
	public void removeAllWalls () {
		
		for (int x = 0; x < mapWidth; ++x) {
			for (int y = 0; y < mapHeight; ++y) {
				removeWall(new Vector2(x,y));
			}
		}
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
		canMove [(int)coord.x, (int)coord.y] = true;
		return true;
	}

	public int getMapWidth(){
		return mapWidth;
	}

	public int getMapHeight(){
		return mapHeight;
	}


	// Assignment 2 adjacent node sensor
	// adjacent node sensor will detect nearby nodes (those nodes within one cell of the given agent)
	public List<Vector2> getNearNodes(Agent a){
		List<Vector2> nearNodes = new List<Vector2> ();

		Vector2 myCell = getCellIndex (a.renderer.bounds.center);
		
		for (int i = -1; i < 2; i++) {
			for(int j = -1; j < 2; j++){
				Vector2 newVec = new Vector2(myCell.x+i, myCell.y+j);

				//if the location is not inbounds, ignore it
				if(!inBounds(newVec))
					continue;

				//if the location is moveable to, add it to the nearNodes list
				if(canMove[(int)newVec.x, (int)newVec.y]){

					//Corner case!
					if(Mathf.Abs(i) == Mathf.Abs(j) && !canMove[(int)newVec.x,(int)myCell.y] && !canMove[(int)myCell.x,(int)newVec.y]){
						continue;
					}

					nearNodes.Add(newVec);

				}
			}
		}
		
		return nearNodes;
	}




	// Flag: Has Dispose already been called? 
	bool disposed = false;
	
	// Public implementation of Dispose pattern callable by consumers. 
	public void Dispose()
	{ 
		Dispose(true);
		GC.SuppressFinalize(this);           
	}
	
	// Protected implementation of Dispose pattern. 
	protected virtual void Dispose(bool disposing)
	{
		if (disposed)
			return; 
		
		if (disposing) {
			removeAllWalls();
		}

		disposed = true;
	}
}
