using UnityEngine;
using System.Collections;

public abstract class MainMenuState : GUIState {

	protected MainMenuFSM fsm;

	public MainMenuState (MainMenuFSM fsm) : base(fsm) {
		this.fsm = fsm;
	}

	public override void displayGraphics () {

		GUI.DrawTexture(new Rect(Screen.width/2 - fsm.mainMenu.Logo.width/2, Screen.height/2 - 200 - fsm.mainMenu.Logo.height, fsm.mainMenu.Logo.width, fsm.mainMenu.Logo.height), fsm.mainMenu.Logo);
	}

}
