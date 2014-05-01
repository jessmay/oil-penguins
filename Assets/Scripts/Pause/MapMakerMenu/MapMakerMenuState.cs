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

		Vector2 center = new Vector2(Screen.width/2, (Screen.height - PlayGameGUI.GUISize)/2);

		label.fontSize = 30;

		GUI.Box(new Rect(center.x - sWidth/4, center.y - 3*(buttonHeight- button.border.top), sWidth/2, buttonHeight),GUIContent.none, box);
		GUI.Label(new Rect(center.x - sWidth/4, center.y - 3*(buttonHeight- button.border.top) + 20, sWidth/2, buttonHeight), "Map Maker", label);

		
		if(GUI.Button(new Rect(center.x - sWidth/4, center.y - 2*(buttonHeight- button.border.top), sWidth/2, buttonHeight),"Save Map", button)) {
			
			finiteStateMachine.changeState(typeof(MapMakerMenuStateSave));
		}


		GUI.enabled = false;
		if(GUI.Button(new Rect(center.x - sWidth/4, center.y - 1 *(buttonHeight- button.border.top), sWidth/2, buttonHeight),"Create New Map", button)) {

		}
		
		
		if(GUI.Button(new Rect(center.x - sWidth/4, center.y + 0 *(buttonHeight- button.border.top), sWidth/2, buttonHeight),"Load Map", button)) {
			
			//finiteStateMachine.changeState(typeof(TempMainMenuStateMapEditor));
		}

		GUI.enabled = true;

		//Unpause
		if(GUI.Button(new Rect(center.x - sWidth/4, center.y + 1 *(buttonHeight- button.border.top), sWidth/2, buttonHeight),"Resume", button)) {
			
			finiteStateMachine.pauseMenu.unPause();
		}

		//Back to main menu
		if(GUI.Button(new Rect(center.x - sWidth/4, center.y + 2 *(buttonHeight- button.border.top), sWidth/2, buttonHeight),"Back to Menu", button)) {
			
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
