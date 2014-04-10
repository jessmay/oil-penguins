using UnityEngine;
using System.Collections;

public class TempMainMenuState : GUIState {

	public TempMainMenuState (TempMainMenuFSM fsm) : base(fsm) { }

	public override string getName () {
		return "Temporary Main Menu State";
	}

	public override void updateGraphics () {}

	public override void displayGraphics () {

		label.fontSize = 30;
		
		GUI.Label(new Rect(Screen.width/2 - width, Screen.height/2 - buttonHeight*3/2, width*2, buttonHeight),"Temp Pause Menu", label);
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2- buttonHeight + button.border.top, sWidth/2, buttonHeight),"AStar Test", button)) {
			
			Application.LoadLevel("AStar");
			finiteStateMachine.pauseMenu.unPause();
		}
		
//		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2, sWidth/2, buttonHeight),"AI Trainer", button)) {
//
//			finiteStateMachine.pauseMenu.unPause();
//			Application.LoadLevel("ANNTrainerOld");
//		}

		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2, sWidth/2, buttonHeight),"Play", button)) {
			
			//finiteStateMachine.pauseMenu.unPause();
			//Application.LoadLevel("PlayGame");

			finiteStateMachine.changeState(typeof(TempMainMenuStateLoadMap), TempMainMenuStateLoadMap.FROM_PLAY_GAME);
		}


		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Map Editor", button)) {

			finiteStateMachine.changeState(typeof(TempMainMenuStateMapEditor));
		}

		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top) *2, sWidth/2, buttonHeight),"Train ANN", button)) {

			finiteStateMachine.changeState(typeof(TempMainMenuStateTrainANN));
		}

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
