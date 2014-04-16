using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class HighScores {

	public static string HighScoresFileName = "HS.txt";

	public static int MAX_SCORES = 10;
	public List<Score> highScores;

	public HighScores() {
		highScores = new List<Score>();
	}

	private HighScores(Score[] scores) {
		highScores = new List<Score>(scores);
	}

	public void addScore(Score score) {

		highScores.Add(score);
		highScores.Sort();

		if(highScores.Count > MAX_SCORES) {
			highScores.RemoveAt(MAX_SCORES);
		}

		File.WriteAllText(Options.HighScoresDirectory +"/"+ HighScoresFileName, save ()); 
	}

	public string save() {
		string contents = "";

		for(int currScore = 0; currScore < highScores.Count; ++currScore) {

			contents += highScores[currScore].save();

			if(currScore != highScores.Count - 1) {
				contents += "\n";
			}
		}

		return contents;
	}

	public static HighScores load(string contents) {

		string[] scoresText = contents.Split(new char[]{'\n'});
		Score[] scores = new Score[scoresText.Length];

		for(int currScore = 0; currScore < scores.Length; ++currScore) {

			scores[currScore] = Score.load(scoresText[currScore]);
		}

		return new HighScores();
	}

}

