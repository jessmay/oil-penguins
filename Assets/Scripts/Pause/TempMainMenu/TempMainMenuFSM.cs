using UnityEngine;
using System.Collections;

public class TempMainMenuFSM : GUIFSM {

	public TempMainMenuFSM(PauseMenu pauseMenu) : base(pauseMenu) {}

	public override System.Type getDefaultState ()
	{
		return typeof (TempMainMenuState);
	}

	public override void initialize ()
	{
		addState(new TempMainMenuState(this));
		addState(new TempMainMenuStateMapEditor(this));
		addState(new TempMainMenuStateLoadMap(this));
		addState(new TempMainMenuStateMapEditorNew(this));
		addState(new TempMainMenuStateTrainANN(this));
		addState(new TempMainMenuStateLoadPop(this));
	}
}
