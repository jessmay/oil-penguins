using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TempMainMenuStateLoadMap : GUIState {


	public static int FROM_PLAY_GAME = 1;
	public static int FROM_MAP_EDITOR = 2;
	public static int FROM_HUMAN_TESTS = 3;

	public TempMainMenuStateLoadMap (TempMainMenuFSM fsm) : base(fsm) { }
	
	public override string getName () {
		return "Temporary Main Menu Map Editor Load";
	}
	
	public override void updateGraphics () {}


	public override void displayGraphics () {

		string path = Options.MapDirectory;

		string fileName = getFileFromDirectory(path, "Select a Map", "Back", 4);

		if(fileName == null) {
			finiteStateMachine.changeToPreviousState();
		}
		else if (!fileName.Equals("")) {
			Options.mapName = fileName;

			if(statusCode == FROM_PLAY_GAME) {
				Application.LoadLevel("PlayGame");
			}
			else if(statusCode == FROM_MAP_EDITOR) {
				Application.LoadLevel("MapEditor");
			}
			else if(statusCode == FROM_HUMAN_TESTS) {
				Application.LoadLevel("AITrainer");
				Options.Testing = true;
			}

			finiteStateMachine.pauseMenu.unPause();
		}
	}
		
	public override void enter () {
		scrollPosition = Vector2.zero;
	}
	
	public override void exit () {}
	
	protected override bool isValidStatus (int statusCode) {
		return (statusCode == FROM_PLAY_GAME || statusCode == FROM_MAP_EDITOR || statusCode == FROM_HUMAN_TESTS);
	}
}
