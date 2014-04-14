using UnityEngine;
using System.Collections;

public class PlayGamePauseStateGameOver : PlayGamePauseState {

	public PlayGamePauseStateGameOver (PlayGamePauseFSM fsm) : base(fsm) { }

	public override string getName () {
		return "Play Game Pause Menu State Game Over";
	}

	public override void updateGraphics () {}

	public override void displayGraphics () {

		//TODO: Center based on screen.height - PlayGameGUI.GUISize
		//Background box
		GUI.Box (new Rect(Screen.width/2 - sWidth/2, Screen.height/2 - sHeight/2, sWidth, sHeight), GUIContent.none, box);
		
		//Title
		label.fontSize = 60;
		GUI.Label(new Rect(Screen.width/2 - sWidth/2, Screen.height/2 - sHeight/2 + 20, sWidth, 70), "Game Over", label);
		//Sub title
		label.fontSize = 40;
		GUI.Label(new Rect(Screen.width/2 - sWidth/2, Screen.height/2 - sHeight/2 + 70, sWidth, 70), "Humans Win", label);

		// Back to menu button
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/2, Screen.height/2 + sHeight/2 - buttonHeight, sWidth, buttonHeight),"Return to Main Menu", button)) {
			
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
