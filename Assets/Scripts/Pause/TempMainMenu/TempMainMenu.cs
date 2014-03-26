using UnityEngine;
using System.Collections;

public class TempMainMenu : PauseMenu {


	protected override GUIFSM createFSM () {
		return new TempMainMenuFSM(this);
	}

	protected override System.Type initialPause () {
		return typeof(TempMainMenuState);
	}

	protected override bool forcedPause() {
		return true;
	}

	protected override void exitPause (){}
	protected override void startPause (){}
}
