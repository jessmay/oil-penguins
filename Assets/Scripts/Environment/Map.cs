/*
Jessica May
Joshua Linge
Map.cs

Updated by Joshua Linge on 2014-03-17
 */

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Map : IDisposable {

	public string name;

	private GameObject wall;

	public int mapWidth {get; private set;}
	public int mapHeight {get; private set;}
	
	public float xSize {get; private set;}
	public float ySize {get; private set;}
	
	//private Bounds bounds;
	private Vector3 center;

	private GameObject[,] board;
	public bool[,] canMove;



	public List<Vector2> HumanSpawnPoints;// {get; private set;}
	public Vector2 ICEMachineLocation;// {get; private set;}
	public Vector2 PenguinSpawn;// {get; private set;}


	//Map colors
	public static Color WallColor = Color.black;
	public static Color HumanSpawnColor = Color.red;
	public static Color PenguinSpawnColor = Color.blue;
	public static Color ICEMachineColor = Color.cyan;
	public static Color EmptyColor = Color.clear;

	public static Vector2 INVALID_LOCATION = -Vector2.one;


	//Construct default map with the given size
	public Map(string n, GameObject w, int width, int height) {

		init(n, w, width, height);

		createBorder();

		ICEMachineLocation = new Vector2(mapWidth/2, mapHeight/2);
		PenguinSpawn = new Vector2(mapWidth/2, mapHeight/2+1);
	}

	//Construct a map based on the given image
	public Map(string n, GameObject w, Texture2D map) {

		//if(Options.play)
			readMap(n, w, map);
		//else
		//	readMapForEditor(n, w, map);
	}

	//Initialize variables
	private void init (string n, GameObject w, int width, int height) {

		name = n;

		wall = w;

		createBoard(width, height);

		center = Vector3.zero;

		BoxCollider2D box = wall.GetComponent<BoxCollider2D>();
		xSize = box.size.x * wall.transform.localScale.x;
		ySize = box.size.y * wall.transform.localScale.y;

//		xSize = wall.renderer.bounds.size.x;
//		ySize = wall.renderer.bounds.size.y;


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
	
	public Vector2 getRandomHumanSpawn() {
		return HumanSpawnPoints[Mathf.FloorToInt(UnityEngine.Random.value*HumanSpawnPoints.Count)];
	}


	public Vector2 getEdgeDirectionVector(Vector2 spawnPoint) {

		Vector2 direction = Vector2.zero;
		
		if(spawnPoint.x < 1)
			direction.x += 1;
		else if (spawnPoint.x > mapWidth-2)
			direction.x -= 1;
		
		if(spawnPoint.y < 1)
			direction.y += 1;
		else if (spawnPoint.y > mapHeight-2)
			direction.y -= 1;

		return direction;
	}

	public Quaternion getSpawnAngle(int spawnIndex) {
		return getSpawnAngle(HumanSpawnPoints[spawnIndex]);
	}

	public Quaternion getSpawnAngle(Vector2 spawnPoint) {

		return Quaternion.LookRotation(Vector3.forward, getEdgeDirectionVector(spawnPoint));
	}


	private void createBoard(int width, int height) {

		mapWidth = width;
		mapHeight = height;

		//Board to hold all walls
		board = new GameObject[mapHeight/2,mapWidth/2];

		//contains whether or not a cell can be moved into based on walls
		canMove = new bool[mapWidth, mapHeight];
		for (int i = 0; i < mapWidth; i++) {
			for(int j = 0; j < mapHeight; j++){
				canMove[i,j] = true;
			}
		}

		HumanSpawnPoints = new List<Vector2>();
		PenguinSpawn = INVALID_LOCATION;
		ICEMachineLocation = INVALID_LOCATION;
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
	private void readMap(string n, GameObject w, Texture2D map) {

		removeAllWalls();

		w.transform.localScale = Vector3.one*0.5f;

		init (n, w, map.width*2, map.height*2);

		w.transform.localScale = Vector3.one;
		
		for (int x = 0; x < map.width; ++x) {
			for (int y = 0; y < map.height; ++y) {

				Color pixel = map.GetPixel(x,y);

				if (pixel == WallColor) {
					addWall(new Vector2(2*x +0.5f, 2*y +0.5f));
//					addWall(new Vector2(2*x +1, 2*y));
//					addWall(new Vector2(2*x, 2*y +1));
//					addWall(new Vector2(2*x +1, 2*y +1));
				}

				else if (pixel == HumanSpawnColor) {

					//Not on the edge of the map
					if((x != 0 && x != map.width-1 && y != 0 && y != map.height-1) && !Options.Testing) {
						//throw new Exception("Invalid map. Human spawn location not on the edge. ("+x +", "+y +")");
					}

					HumanSpawnPoints.Add(new Vector2(2*x+0.5f,2*y+0.5f));
				}

				else if (pixel == PenguinSpawnColor) {

					if(PenguinSpawn != INVALID_LOCATION) {
						throw new Exception("Invalid map. Multiple penguin spawn points. ("+PenguinSpawn.x +", "+PenguinSpawn.y +") ("+x +", "+y +")");
					}

					PenguinSpawn = new Vector2(2*x+0.5f,2*y+0.5f);
				}

				else if (pixel == ICEMachineColor) {
					
					if(ICEMachineLocation != INVALID_LOCATION) {
						throw new Exception("Invalid map. Multiple ICE Machine locations. ("+ICEMachineLocation.x +", "+ICEMachineLocation.y +") ("+x +", "+y +")");
					}
					
					ICEMachineLocation = new Vector2(2*x+0.5f,2*y+0.5f);
				}
			}
		}
	}

	//Place walls based off given map texture
	private void readMapForEditor(string n, GameObject w, Texture2D map) {
		
		removeAllWalls();
		
		w.transform.localScale = Vector3.one*0.5f;

		init (n, w, map.width, map.height);
		
		//createBoard(map.width, map.height);
		
		for (int x = 0; x < map.width; ++x) {
			for (int y = 0; y < map.height; ++y) {
				
				Color pixel = map.GetPixel(x,y);
				
				if (pixel == WallColor) {
					addWall(new Vector2(x, y));
				}
				
				else if (pixel == HumanSpawnColor) {
					
					//Not on the edge of the map
					if((x != 0 && x != map.width-1 && y != 0 && y != map.height-1) && !Options.Testing) {
						//throw new Exception("Invalid map. Human spawn location not on the edge. ("+x +", "+y +")");
					}
					
					HumanSpawnPoints.Add(new Vector2(x,y));
				}
				
				else if (pixel == PenguinSpawnColor) {
					
					if(PenguinSpawn != INVALID_LOCATION) {
						throw new Exception("Invalid map. Multiple penguin spawn points. ("+PenguinSpawn.x +", "+PenguinSpawn.y +") ("+x +", "+y +")");
					}
					
					PenguinSpawn = new Vector2(x,y);
				}
				
				else if (pixel == ICEMachineColor) {
					
					if(ICEMachineLocation != INVALID_LOCATION) {
						throw new Exception("Invalid map. Multiple ICE Machine locations. ("+ICEMachineLocation.x +", "+ICEMachineLocation.y +") ("+x +", "+y +")");
					}
					
					ICEMachineLocation = new Vector2(x,y);
				}
			}
		}
	}

	public Texture2D saveMap () {

		Texture2D map = new Texture2D(mapWidth/2, mapHeight/2);

		for (int x = 0; x < map.width; ++x) {
			for (int y = 0; y < map.height; ++y) {
				
				if (board[y,x] != null) {
					map.SetPixel(x,y, WallColor);
				}
				else {
					map.SetPixel(x,y, EmptyColor);
				}
			}
		}

		if(PenguinSpawn != INVALID_LOCATION)
			map.SetPixel((int)PenguinSpawn.x/2, (int)PenguinSpawn.y/2, PenguinSpawnColor);

		if(ICEMachineLocation != INVALID_LOCATION)
			map.SetPixel((int)ICEMachineLocation.x/2, (int)ICEMachineLocation.y/2, ICEMachineColor);

		foreach (Vector2 loc in HumanSpawnPoints) {
			map.SetPixel((int)loc.x/2, (int)loc.y/2, HumanSpawnColor);
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
		if(!inBounds(coord) || board[(int)coord.y/2,(int)coord.x/2] != null)
			return false;

		placeWall(coord);
		return true;
	}

	//Create and place a wall at the given cell index
	private void placeWall(Vector2 coord) {
		Vector3 placeLocation = cellIndexToWorld(coord);
		GameObject w = GameObject.Instantiate (wall, placeLocation, wall.transform.rotation) as GameObject;
		board[(int)coord.y/2,(int)coord.x/2] = w;


		canMove [Mathf.FloorToInt(coord.x), Mathf.FloorToInt(coord.y)] = false;
		canMove [Mathf.FloorToInt(coord.x), Mathf.CeilToInt(coord.y)] = false;
		canMove [Mathf.CeilToInt(coord.x), Mathf.FloorToInt(coord.y)] = false;
		canMove [Mathf.CeilToInt(coord.x), Mathf.CeilToInt(coord.y)] = false;

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
		if (!inBounds(coord) || board[(int)coord.y/2,(int)coord.x/2] == null) {
			return false;
		}
		GameObject w = board[(int)coord.y/2,(int)coord.x/2];
		GameObject.Destroy(w);

		board[(int)coord.y/2,(int)coord.x/2] = null;
		canMove [(int)coord.x, (int)coord.y] = true;
		return true;
	}

	public bool hasWall(Vector3 coord) {
		return hasWall (getCellIndex(coord));
	}

	public bool hasWall(Vector2 coord) {
		return board[(int)coord.y/2,(int)coord.x/2] != null;
	}

	public int getMapWidth(){
		return mapWidth;
	}

	public int getMapHeight(){
		return mapHeight;
	}




	public Vector2 getMovableCoord(Vector2 coord, float radius) {
		
		int[] dx = {0, 1, 0, -1, 1, 1, -1, -1};
		int[] dy = {1, 0, -1, 0, 1, -1, 1, -1};
		
		Vector2 newCoord = coord;
		Vector2 currCellWorld = cellIndexToWorld(getCellIndex(coord));

		HashSet<Vector2> cellSeen = new HashSet<Vector2>();
		cellSeen.Add(getCellIndex(coord));
		
		for (int currDir = 0; currDir < dx.Length; ++currDir) {
			
			Vector2 direction = new Vector2(dx[currDir], dy[currDir]);
			Vector2 testLoc = newCoord + direction * radius;
			Vector2 testCell = getCellIndex(testLoc);
			
			if(cellSeen.Contains(testCell))
				continue;

			cellSeen.Add(testCell);
			
			if(!inBounds(testCell) || !canMove[(int)testCell.x,(int)testCell.y]) {
				
				Vector2 edge = new Vector2(currCellWorld.x + direction.x * (xSize/2), currCellWorld.y + direction.y * (ySize/2));
				
				Vector2 diff = new Vector2((testLoc.x - edge.x) * Mathf.Abs(dx[currDir]), (testLoc.y - edge.y) * Mathf.Abs(dy[currDir]));
				newCoord -= diff;
				
				//Debug.Log("Diff for direction["+dx[currDir]+"]["+dy[currDir]+"]: "+diff);
			}
		}
		
		//Debug.Log("Original: "+coord +" New: "+newCoord);
		
		return newCoord;
	}






	//public static string MapDirectory;

	//Save the current map to the given file name.
	public void saveMap (string mapName) {
		
		Debug.Log("Saving Map " +mapName);
		
		if(!Directory.Exists(Options.MapDirectory)) 
			Directory.CreateDirectory(Options.MapDirectory);
		
		byte[] bytes = saveMap().EncodeToPNG();
		File.WriteAllBytes(Options.MapDirectory +"/" + mapName +".png", bytes);
		
	}

	//Load a map from the given file name.
	public static Map loadMap (string mapName, GameObject Wall) {

		//string MapDirectory = Application.dataPath + "/../Maps";

		Debug.Log("Loading Map " +mapName);

		string fileName = Options.MapDirectory +"/"+ mapName +".png";

		if(!File.Exists(fileName)) {
			Debug.LogWarning("Map "+mapName +" could not be found.");
			mapName = "Default";
		}
			
		byte[] bytes = File.ReadAllBytes(Options.MapDirectory +"/"+ mapName +".png");
		Texture2D mapImage = new Texture2D(1,1);
		mapImage.LoadImage(bytes);
		
		return new Map(mapName, Wall, mapImage);
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
