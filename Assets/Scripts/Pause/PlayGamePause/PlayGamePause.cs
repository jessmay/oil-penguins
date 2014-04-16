using UnityEngine;
using System.Collections;

public class PlayGamePause : PauseMenu {

	public GameMap gameMap;
	public WaveManager waveManager;

	protected override GUIFSM createFSM () {
		return new PlayGamePauseFSM(this);
	}

	protected override System.Type initialPause () {
		return null;
	}

	protected override bool forcedPause() {
		return false;
	}
	
	protected override void startPause (){}

	protected override void exitPause (){}

}
