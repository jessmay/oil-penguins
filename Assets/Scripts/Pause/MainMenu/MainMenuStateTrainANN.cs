using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MainMenuStateTrainANN : MainMenuState {

	public MainMenuStateTrainANN (MainMenuFSM fsm) : base(fsm) { }
	
	public override string getName () {
		return "Main Menu Train ANN";
	}
	
	public override void updateGraphics () {}


	public override void displayGraphics () {
		base.displayGraphics();
		
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - 2 * (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Create New Population", button)) {

			finiteStateMachine.changeState(typeof(MainMenuStateSelectGenome));
				
		}
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - 1 * (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Load Population", button)) {
			
			finiteStateMachine.changeState(typeof(MainMenuStateLoadPop));
		}
		
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + 0 * (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Back", button)) {
			
			finiteStateMachine.changeState(typeof(MainMenuStateMain));
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
