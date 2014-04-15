using UnityEngine;
using System.Collections;

public class PlayGamePauseStateGameOver : PlayGamePauseState {

	public PlayGamePauseStateGameOver (PlayGamePauseFSM fsm) : base(fsm) { }

	public override string getName () {
		return "Play Game Pause Menu State Game Over";
	}

	public override void updateGraphics () {}

	public override void displayGraphics () {

		int wWidth = 350;
		int wHeight = 380;

		Vector2 center = new Vector2(Screen.width/2, (Screen.height - PlayGameGUI.GUISize)/2);

		//Background box
		GUI.Box (new Rect(center.x - wWidth/2, center.y - wHeight/2, wWidth, wHeight), GUIContent.none, box);
		
		//Title
		label.fontSize = 60;
		GUI.Label(new Rect(center.x - wWidth/2, center.y - wHeight/2 + 20, wWidth, 70), "Game Over", label);
		//Sub title
		label.fontSize = 40;
		GUI.Label(new Rect(center.x - wWidth/2, center.y - wHeight/2 + 90, wWidth, 70), "Humans Win", label);

		label.fontSize = 25;

		label.alignment = TextAnchor.UpperLeft;
		GUI.Label(new Rect(center.x - wWidth*4/10, center.y - wHeight/2 + 160, wWidth*4/5, 70), "Waves completed:", label);
		GUI.Label(new Rect(center.x - wWidth*4/10, center.y - wHeight/2 + 200, wWidth*4/5, 70), "Humans killed:", label);
		GUI.Label(new Rect(center.x - wWidth*4/10, center.y - wHeight/2 + 240, wWidth*4/5, 70), "Time played:", label);

		float playTime = (Time.time - gameMap.gameStartTime);


		label.alignment = TextAnchor.UpperRight;
		GUI.Label(new Rect(center.x - wWidth*4/10, center.y - wHeight/2 + 160, wWidth*4/5, 70), (waveManager.waveNumber-1).ToString(), label);
		GUI.Label(new Rect(center.x - wWidth*4/10, center.y - wHeight/2 + 200, wWidth*4/5, 70), gameMap.humansKilled.ToString(), label);
		GUI.Label(new Rect(center.x - wWidth*4/10, center.y - wHeight/2 + 240, wWidth*4/5, 70), string.Format("{0:00}:{1:00}", ((int)playTime)/60, Mathf.Round (playTime)%60), label);


		label.alignment = TextAnchor.UpperCenter;

		// Back to menu button
		if(GUI.Button(new Rect(center.x - wWidth/2, center.y + wHeight/2 - buttonHeight, wWidth, buttonHeight),"Return to Main Menu", button)) {
			
			Application.LoadLevel("StartMenu");
			finiteStateMachine.pauseMenu.unPause();
		}

		/*
		label.fontSize = 30;
		GUI.Box(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - (buttonHeight - button.border.top)*2, sWidth/2, buttonHeight), GUIContent.none, box);

		GUI.Label(new Rect(Screen.width/2 - width, Screen.height/2 - buttonHeight*3/2, width*2, buttonHeight),"Paused", label);
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2- buttonHeight + button.border.top, sWidth/2, buttonHeight),"Resume", button)) {
			
			finiteStateMachine.pauseMenu.unPause();
		}

		GUI.enabled = false;
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2, sWidth/2, buttonHeight),"Save", button)) {
			
			//finiteStateMachine.pauseMenu.unPause();
			//Application.LoadLevel("PlayGame");

			//finiteStateMachine.changeState(typeof(TempMainMenuStateLoadMap), TempMainMenuStateLoadMap.FROM_PLAY_GAME);
		}
		GUI.enabled = true;

		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Return to Main Menu", button)) {

			Application.LoadLevel("StartMenu");
			finiteStateMachine.pauseMenu.unPause();
		}*/

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
