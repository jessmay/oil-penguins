using UnityEngine;
using System;
using System.Collections;

public class TempMainMenuStateMapEditorNew : GUIState {

	public TempMainMenuStateMapEditorNew (TempMainMenuFSM fsm) : base(fsm) { }
	
	public override string getName () {
		return "Temporary Main Menu Map Editor New Map";
	}
	
	public override void updateGraphics () {}

	//int mapWidth;
	//int mapHeight;

	string tempWidth;
	string tempHeight;

	public override void displayGraphics () {


		label.fontSize = 30;
		
		//Menu title
		GUI.Box(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - 2*(buttonHeight- button.border.top), sWidth/2, buttonHeight),GUIContent.none, box);
		GUI.Label(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - 2*(buttonHeight- button.border.top) + 20, sWidth/2, buttonHeight), "Map Size:", label);


		finiteStateMachine.pauseMenu.skin.textField.fontSize = 30;
		finiteStateMachine.pauseMenu.skin.textField.alignment = TextAnchor.MiddleCenter;

		tempWidth = GUI.TextField (new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - height, sWidth/4 - 20, height), tempWidth, 2, finiteStateMachine.pauseMenu.skin.textField);

//		uint tempNum;
//
//		if (uint.TryParse(tempString, out tempNum)){
//			mapWidth = Convert.ToInt32(tempNum);
//		}
//		else {
//			Debug.Log(tempString +"is not a valid width for the map.");
//		}


		GUI.Label(new Rect(Screen.width/2 -20, Screen.height/2 - height, 40, height), "x", label);

		tempHeight = GUI.TextField (new Rect(Screen.width/2 + 20, Screen.height/2 - height, sWidth/4 - 20, height), tempHeight, 2, finiteStateMachine.pauseMenu.skin.textField);
		
//		if (uint.TryParse(tempString, out tempNum)){
//			mapHeight = Convert.ToInt32(tempNum);
//		}
//		else {
//			Debug.Log(tempString +"is not a valid height for the map.");
//		}

		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2, sWidth/2, buttonHeight),"Create Map", button)) {

			uint mapWidth;
			uint mapHeight;
			if (!uint.TryParse(tempWidth, out mapWidth) || !uint.TryParse(tempHeight, out mapHeight)){
				Debug.Log(tempWidth +" by " +tempHeight +" are not valid map dimensions.");
			}

			else {
				if (mapWidth < 10 || mapHeight < 10) {
					Debug.Log("The map must have a minimum size of 10 x 10");
				}
				else {
					finiteStateMachine.pauseMenu.unPause();
					Options.mapSize = new Vector2(mapWidth, mapHeight);
					Application.LoadLevel("MapEditor");
				}
			}
		}
		
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Back", button)) {

			finiteStateMachine.changeState(typeof(TempMainMenuState));
		}

	}
		
	public override void enter () {
		//mapWidth = 20;
		//mapHeight = 20;

		tempWidth = "20";
    	tempHeight = "20";
	}
	
	public override void exit () {}
	
	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
