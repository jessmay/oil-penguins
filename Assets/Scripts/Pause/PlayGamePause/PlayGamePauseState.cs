using UnityEngine;
using System.Collections;

public abstract class PlayGamePauseState : GUIState {
	
	protected GameMap gameMap;
	protected WaveManager waveManager;

	public PlayGamePauseState (PlayGamePauseFSM fsm) : base(fsm) { 
		gameMap = fsm.pauseMenu.gameMap;
		waveManager = fsm.pauseMenu.waveManager;
	}

}
