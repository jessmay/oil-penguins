using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuStateHighScores : MainMenuState {

	public MainMenuStateHighScores (MainMenuFSM fsm) : base(fsm) { }

	public override string getName () {
		return "Main Menu High Scores State";
	}

	public override void updateGraphics () {}

	public override void displayGraphics () {
		base.displayGraphics();


		
		int startWidth = Screen.width/2 - sWidth/2;
		int startHeight = Screen.height/2 - sHeight/2;
		
		//Background box
		GUI.Box (new Rect (startWidth, startHeight, sWidth, sHeight), GUIContent.none, box);

		label.fontSize = 50;
		GUI.Label(new Rect(startWidth, startHeight + 5, sWidth, 70), "High Scores", label);


		label.fontSize = 30;
		GUI.Label(new Rect(startWidth, startHeight + 50, sWidth, 50), "   Waves  |  Humans  |   Time   |   Map    ", label);

		List<Score> scores = Options.highScores.highScores;

		label.fontSize = 20;
		for(int currScore = 0; currScore < scores.Count; ++currScore) {

			GUI.Label(new Rect(startWidth + 150, startHeight + 90 + 30* currScore, 20, 30), scores[currScore].waveReached.ToString(), label);
			GUI.Label(new Rect(startWidth + 280, startHeight + 90 + 30* currScore, 20, 30), scores[currScore].killedHumans.ToString(), label);

			string timePlayed = string.Format("{0:00}:{1:00}", ((int)scores[currScore].timePlayed)/60, ((int)scores[currScore].timePlayed)%60);

			GUI.Label(new Rect(startWidth + 350, startHeight + 90 + 30* currScore, 100, 50), timePlayed, label);

			GUI.Label(new Rect(startWidth + 480, startHeight + 90 + 30* currScore, 100, 50), scores[currScore].mapName, label);

		}

		//Unpause
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/2, (Screen.height/2)+((sHeight/2)-buttonHeight), sWidth, buttonHeight), "Back to the Menu", button)) {
			
			finiteStateMachine.changeState(typeof(MainMenuStateMain));
		}

//		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + 1 * (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Back to the Menu", button)) {
//
//			finiteStateMachine.changeState(typeof(MainMenuStateMain));
//		}

	}

	public override void enter () {
		Options.mapName = null;
		Options.Testing = false;
		Options.play = false;

		Options.populationName = null;
	}

	public override void exit () {}

	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
