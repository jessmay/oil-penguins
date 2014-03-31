using UnityEngine;
using System.Collections;

public class MapMakerMenuFSM : GUIFSM {

	public MapMakerMenuFSM(PauseMenu pauseMenu) : base(pauseMenu) {}

	public override System.Type getDefaultState ()
	{
		return typeof (MapMakerMenuState);
	}

	public override void initialize ()
	{
		addState(new MapMakerMenuState(this));
		addState(new MapMakerMenuStateSave(this));
	}
}
