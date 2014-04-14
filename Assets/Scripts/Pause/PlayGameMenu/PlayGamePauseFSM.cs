using UnityEngine;
using System.Collections;

public class PlayGamePauseFSM : GUIFSM {

	public new PlayGamePause pauseMenu;

	public PlayGamePauseFSM(PlayGamePause pauseMenu) : base(pauseMenu) {
		this.pauseMenu = pauseMenu;
	}

	public override System.Type getDefaultState ()
	{
		return typeof (PlayGamePauseStateMain);
	}

	public override void initialize ()
	{
		addState(new PlayGamePauseStateMain(this));
		addState(new PlayGamePauseStateGameOver(this));

//		addState(new TempMainMenuStateLoadMap(this));
//		addState(new TempMainMenuStateMapEditorNew(this));
//		addState(new TempMainMenuStateTrainANN(this));
//		addState(new TempMainMenuStateLoadPop(this));
//		addState(new TempMainMenuStateSelectGenome(this));

	}
}
