using UnityEngine;
using System.Collections;

public class PlayGamePauseStateMain : PlayGamePauseState {

	public PlayGamePauseStateMain (PlayGamePauseFSM fsm) : base(fsm) { }

	public override string getName () {
		return "Play Game Pause Menu State Main";
	}

	public override void updateGraphics () {}

	public override void displayGraphics () {

		Vector2 center = new Vector2(Screen.width/2, (Screen.height - PlayGameGUI.GUISize)/2);

		label.fontSize = 30;
		GUI.Box(new Rect(center.x - sWidth/4, center.y - (buttonHeight - button.border.top)*2, sWidth/2, buttonHeight), GUIContent.none, box);

		GUI.Label(new Rect(center.x - width, center.y - buttonHeight*3/2, width*2, buttonHeight),"Paused", label);
		
		if(GUI.Button(new Rect(center.x - sWidth/4, center.y - buttonHeight + button.border.top, sWidth/2, buttonHeight),"Resume", button)) {
			
			finiteStateMachine.pauseMenu.unPause();
		}

		GUI.enabled = false;
		if(GUI.Button(new Rect(center.x - sWidth/4, center.y, sWidth/2, buttonHeight),"Save", button)) {
			
			//finiteStateMachine.pauseMenu.unPause();
			//Application.LoadLevel("PlayGame");

			//finiteStateMachine.changeState(typeof(TempMainMenuStateLoadMap), TempMainMenuStateLoadMap.FROM_PLAY_GAME);
		}
		GUI.enabled = true;

		if(GUI.Button(new Rect(center.x - sWidth/4, center.y + (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Return to Main Menu", button)) {

			Application.LoadLevel("StartMenu");

			if(gameMap.humansKilled > 0)
				Options.highScores.addScore(new Score(waveManager.waveNumber, gameMap.humansKilled, Time.time - gameMap.gameStartTime, gameMap.map.name, System.DateTime.Now));

			finiteStateMachine.pauseMenu.unPause();
		}

//		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top) *2, sWidth/2, buttonHeight),"Train ANN", button)) {
//
//			finiteStateMachine.changeState(typeof(TempMainMenuStateTrainANN));
//		}

	}

	public override void enter () {}

	public override void exit () {}

	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
