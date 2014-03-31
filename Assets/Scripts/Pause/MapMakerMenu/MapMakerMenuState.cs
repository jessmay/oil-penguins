using UnityEngine;
using System.Collections;

public class MapMakerMenuState : GUIState {

	public MapMakerMenuState (MapMakerMenuFSM fsm) : base(fsm) { }

	public override string getName () {
		return "Map Maker Menu State";
	}

	public override void updateGraphics () {}

	public override void displayGraphics () {

		//Save
		//Create new
		//Load
		//Return

		label.fontSize = 30;
		
//		GUI.Box(new Rect(Screen.width/2 - width, Screen.height/2 - buttonHeight*3/2, width*2, buttonHeight), GUIContent.none, box);
//		GUI.Label(new Rect(Screen.width/2 - width, Screen.height/2 - buttonHeight*3/2, width*2, buttonHeight),"Map Maker", label);

		GUI.Box(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - 2*(buttonHeight- button.border.top), sWidth/2, buttonHeight),GUIContent.none, box);
		GUI.Label(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - 2*(buttonHeight- button.border.top) + 20, sWidth/2, buttonHeight), "Map Maker", label);

		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2- buttonHeight + button.border.top, sWidth/2, buttonHeight),"Save Map", button)) {
			
			finiteStateMachine.changeState(typeof(MapMakerMenuStateSave));
		}
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2, sWidth/2, buttonHeight),"Create New Map", button)) {

		}
		
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Load Map", button)) {
			
			//finiteStateMachine.changeState(typeof(TempMainMenuStateMapEditor));
		}

		//Unpause
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top) *2, sWidth/2, buttonHeight),"Resume", button)) {
			
			finiteStateMachine.pauseMenu.unPause();
		}

		//Back to main menu
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top) *3, sWidth/2, buttonHeight),"Back to menu", button)) {
			
			finiteStateMachine.pauseMenu.unPause();
			Application.LoadLevel("StartMenu");
		}

	}

	public override void enter () {}

	public override void exit () {}

	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
