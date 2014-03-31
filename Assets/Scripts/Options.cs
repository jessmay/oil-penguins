using UnityEngine;
using System.Collections;

public class Options {

	public static string mapName;
	public static GameMap gameMap;
	
	public static Vector2 mapSize;

	public static string MapDirectory = Application.dataPath + "/../Maps";

	static Options() {
		//mapName = "NotSure";//"FourCorners";//"Default";
	}

}
