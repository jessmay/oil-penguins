using UnityEngine;
using System.Collections;

public class MapMakerMenu : PauseMenu {


	protected override GUIFSM createFSM () {
		return new MapMakerMenuFSM(this);
	}

	protected override System.Type initialPause () {
		return null;
	}

	protected override bool forcedPause() {
		return false;
	}

	protected override void exitPause (){}
	protected override void startPause (){}
}
