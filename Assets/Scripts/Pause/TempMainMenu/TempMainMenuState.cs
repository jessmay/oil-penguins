﻿using UnityEngine;
using System.Collections;

public class TempMainMenuState : GUIState {

	public TempMainMenuState (TempMainMenuFSM fsm) : base(fsm) { }

	public override string getName () {
		return "Temporary Main Menu State";
	}

	public override void updateGraphics () {}

	public override void displayGraphics () {

		label.fontSize = 30;
		
		//GUI.color = Color.white;
		GUI.Label(new Rect(Screen.width/2 - width, Screen.height/2 - buttonHeight*3/2, width*2, buttonHeight),"Temp Pause Menu", label);
		
		// Unpause Button
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2- buttonHeight + button.border.top, sWidth/2, buttonHeight),"AStar Test", button)) {
			
			Application.LoadLevel("AStar");
			finiteStateMachine.pauseMenu.unPause();
		}
		
		// Quit Button
		// Ignored if in editor
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2, sWidth/2, buttonHeight),"AI Trainer", button)) {

			finiteStateMachine.pauseMenu.unPause();
			Application.LoadLevel("ANNTrainer");
		}


		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Map Editor", button)) {

			finiteStateMachine.changeState(typeof(TempMainMenuStateMapEditor));
			//finiteStateMachine.pauseMenu.unPause();
			//Application.LoadLevel("MapEditor");
		}

		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top) *2, sWidth/2, buttonHeight),"Human Testing", button)) {

			finiteStateMachine.changeState(typeof(TempMainMenuStateLoad), TempMainMenuStateLoad.FROM_HUMAN_TESTS);

			//Options.mapName = "NotSure";
			//finiteStateMachine.pauseMenu.unPause();
			//Application.LoadLevel("TestScene");
		}

	}

	public override void enter () {
		Options.mapName = null;
	}

	public override void exit () {}

	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}