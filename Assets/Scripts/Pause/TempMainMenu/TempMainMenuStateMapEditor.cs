﻿using UnityEngine;
using System.Collections;

public class TempMainMenuStateMapEditor : GUIState {

	public TempMainMenuStateMapEditor (TempMainMenuFSM fsm) : base(fsm) { }
	
	public override string getName () {
		return "Temporary Main Menu Map Editor";
	}
	
	public override void updateGraphics () {}
	
	public override void displayGraphics () {

		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2- buttonHeight + button.border.top, sWidth/2, buttonHeight),"Create New", button)) {

			finiteStateMachine.changeState(typeof(TempMainMenuStateMapEditorNew));
		}
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2, sWidth/2, buttonHeight),"Load Map", button)) {

			finiteStateMachine.changeState(typeof(TempMainMenuStateLoadMap), TempMainMenuStateLoadMap.FROM_MAP_EDITOR);
		}
		
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Back", button)) {

			finiteStateMachine.changeState(typeof(TempMainMenuState));
		}

	}
		
	public override void enter () {}
	
	public override void exit () {}
	
	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
