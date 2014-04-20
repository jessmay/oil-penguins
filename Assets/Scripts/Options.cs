using UnityEngine;
using System.IO;
using System.Collections;

public class Options {

	public static bool play;
	public static string mapName;
	public static GameMap gameMap;

	public static GeneticAlgorithmMultiTesting geneticAlgorithm;
	public static string populationName;
	public static bool Testing;
	public static string genomeType;
	
	public static Vector2 mapSize;

	public static string MapDirectory = Application.dataPath + "/../Maps";
	public static string GADirectory = Application.dataPath + "/../GA";
	public static string GenomeDirectory = GADirectory +"/Genomes";

	public static string HighScoresDirectory = Application.dataPath + "/../HS";
	public static HighScores highScores;


	static Options() {
		mapName = "NewMap";
		Testing = false;
		play = false;

		if(File.Exists(HighScoresDirectory +"/"+ HighScores.HighScoresFileName))
			highScores = HighScores.load (File.ReadAllText(HighScoresDirectory +"/"+ HighScores.HighScoresFileName));
		else {
			highScores = new HighScores();
			Directory.CreateDirectory(HighScoresDirectory);
		}
	}

}
