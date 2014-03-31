﻿using UnityEngine;
using System.IO;
using System.Collections;

public class MapMakerMenuStateSave : GUIState {

	string fileName;

	public MapMakerMenuStateSave (MapMakerMenuFSM fsm) : base(fsm) { }

	public override string getName () {
		return "Map Maker Menu State Save";
	}

	public override void updateGraphics () {}

	public override void displayGraphics () {

		//Save
		//Create new
		//Load
		//Return

		label.fontSize = 30;
		
		GUI.Box(new Rect(Screen.width/2 - sWidth/4, Screen.height/2- buttonHeight + button.border.top, sWidth/2, buttonHeight), GUIContent.none, box);
		GUI.Label(new Rect(Screen.width/2 - sWidth/4, Screen.height/2- buttonHeight + button.border.top + 20, sWidth/2, buttonHeight),"Map name:", label);

		// Get file name here

		fileName = GUI.TextField (new Rect(Screen.width/2 - sWidth/4 , Screen.height/2 , sWidth/2 - 20, buttonHeight), fileName, 50, finiteStateMachine.pauseMenu.skin.textField);

		bool fileExists = File.Exists(Options.MapDirectory + "/" +fileName);
		if(fileExists) {
			GUI.Label(new Rect(Screen.width/2 + sWidth/4 -20, Screen.height/2, 40, height), "X", label);
		}
		
		GUI.enabled = !fileExists && !fileName.Equals("");
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Save", button)) {

			Options.gameMap.map.saveMap(fileName);
			finiteStateMachine.pauseMenu.unPause();
		}
		GUI.enabled = true;

		//Unpause
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top) *2, sWidth/2, buttonHeight),"Back", button)) {
			
			finiteStateMachine.changeState(typeof(MapMakerMenuState));
		}

	}

	public override void enter () {
		fileName = "";
	}

	public override void exit () {}

	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
