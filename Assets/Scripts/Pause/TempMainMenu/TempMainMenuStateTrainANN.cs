using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TempMainMenuStateTrainANN : GUIState {

	public TempMainMenuStateTrainANN (TempMainMenuFSM fsm) : base(fsm) { }
	
	public override string getName () {
		return "Temporary Main Menu Train ANN";
	}
	
	public override void updateGraphics () {}


	public override void displayGraphics () {

		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2- buttonHeight + button.border.top, sWidth/2, buttonHeight),"Create New Population", button)) {

			//Options.Testing = true;
			//Options.mapName = "TrainingMap";
			//Application.LoadLevel("TestScene");
			//finiteStateMachine.pauseMenu.unPause();
			finiteStateMachine.changeState(typeof(TempMainMenuStateLoadMap), TempMainMenuStateLoadMap.FROM_HUMAN_TESTS);
		}
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2, sWidth/2, buttonHeight),"Load Population", button)) {
			
			finiteStateMachine.changeState(typeof(TempMainMenuStateLoadPop));
		}
		
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Back", button)) {
			
			finiteStateMachine.changeState(typeof(TempMainMenuState));
		}

//		string path = Options.GADirectory;
//
//		string fileName = getFileFromDirectory(path, "Select a population", "Back", 4);
//
//		if(fileName == null) {
//			finiteStateMachine.changeToPreviousState();
//		}
//		else if (!fileName.Equals("")) {
//			Options.mapName = fileName;
//
//			if(statusCode == FROM_MAP_EDITOR)
//				Application.LoadLevel("MapEditor");
//
//			else if(statusCode == FROM_HUMAN_TESTS) {
//				Application.LoadLevel("TestScene");
//				Options.Testing = true;
//			}
//
//			finiteStateMachine.pauseMenu.unPause();
//		}
	}
		
	public override void enter () {
		scrollPosition = Vector2.zero;
	}
	
	public override void exit () {}
	
	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
