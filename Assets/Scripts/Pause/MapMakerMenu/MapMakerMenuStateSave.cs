using UnityEngine;
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

		Vector2 center = new Vector2(Screen.width/2, (Screen.height - PlayGameGUI.GUISize)/2);

		label.fontSize = 30;
		
		GUI.Box(new Rect(center.x - sWidth/4, center.y - 2 *(buttonHeight- button.border.top), sWidth/2, buttonHeight), GUIContent.none, box);
		GUI.Label(new Rect(center.x - sWidth/4, center.y - 2 *(buttonHeight- button.border.top) + 20, sWidth/2, buttonHeight),"Map name:", label);

		// Get file name here

		bool fileExists = File.Exists(Options.MapDirectory + "/" +fileName +".png");

		fileName = GUI.TextField (new Rect(center.x - sWidth/4, center.y - 1 *(buttonHeight- button.border.top) + button.border.top, sWidth/2 - (fileExists || fileName.Equals("")? 40:0), buttonHeight - 2*button.border.top), fileName, 50, finiteStateMachine.pauseMenu.skin.textField);

		if(fileExists || fileName.Equals("")) {
			GUI.Label(new Rect(center.x + sWidth/4 -40, center.y - 1 *(buttonHeight- button.border.top) + 20, 40, height), "X", label);
		}
		
		GUI.enabled = !fileName.Equals("");
		if(GUI.Button(new Rect(center.x - sWidth/4, center.y - 0 *(buttonHeight- button.border.top), sWidth/2, buttonHeight), (fileExists? "Save over":"Save"), button)) {

			Options.gameMap.map.saveMap(fileName);
			finiteStateMachine.pauseMenu.unPause();
		}
		GUI.enabled = true;

		//Unpause
		if(GUI.Button(new Rect(center.x - sWidth/4, center.y + 1 *(buttonHeight- button.border.top), sWidth/2, buttonHeight),"Back", button)) {
			
			finiteStateMachine.changeState(typeof(MapMakerMenuState));
		}

	}

	public override void enter () {
		fileName = Options.mapName == null? "": Options.mapName;
	}

	public override void exit () {}

	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
