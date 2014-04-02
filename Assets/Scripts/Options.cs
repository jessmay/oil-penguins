using UnityEngine;
using System.Collections;

public class Options {

	public static string mapName;
	public static GameMap gameMap;

	public static string populationName;
	public static bool Testing;
	
	public static Vector2 mapSize;

	public static string MapDirectory = Application.dataPath + "/../Maps";
	public static string GADirectory = Application.dataPath + "/../GA";
	public static string GenomeDirectory = GADirectory +"/Genomes";

	static Options() {
		mapName = "TrainingMap";
		Testing = false;
	}

}
