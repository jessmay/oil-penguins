using UnityEngine;
using System.Collections;

public class MainMenuStateMapEditor : MainMenuState {

	public MainMenuStateMapEditor (MainMenuFSM fsm) : base(fsm) { }
	
	public override string getName () {
		return "Main Menu Map Editor";
	}
	
	public override void updateGraphics () {}
	
	public override void displayGraphics () {
		base.displayGraphics();

		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2- buttonHeight + button.border.top, sWidth/2, buttonHeight),"Create New", button)) {

			finiteStateMachine.changeState(typeof(MainMenuStateMapEditorNew));
		}
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2, sWidth/2, buttonHeight),"Load Map", button)) {

			finiteStateMachine.changeState(typeof(MainMenuStateLoadMap), MainMenuStateLoadMap.FROM_MAP_EDITOR);
		}
		
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Back", button)) {

			finiteStateMachine.changeState(typeof(MainMenuStateMain));
		}

	}
		
	public override void enter () {}
	
	public override void exit () {}
	
	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
