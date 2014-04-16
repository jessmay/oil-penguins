using UnityEngine;
using System.Collections;

public class MainMenu : PauseMenu {

	public Texture2D Logo;

	protected override GUIFSM createFSM () {
		return new MainMenuFSM(this);
	}

	protected override System.Type initialPause () {
		return typeof(MainMenuStateMain);
	}

	protected override bool forcedPause() {
		return true;
	}

	protected override void exitPause (){}
	protected override void startPause (){}
}
