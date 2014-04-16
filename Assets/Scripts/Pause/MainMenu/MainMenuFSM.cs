using UnityEngine;
using System.Collections;

public class MainMenuFSM : GUIFSM {

	public MainMenu mainMenu;

	public MainMenuFSM(MainMenu pauseMenu) : base(pauseMenu) {
		this.mainMenu = pauseMenu;
	}

	public override System.Type getDefaultState ()
	{
		return typeof (MainMenuStateMain);
	}

	public override void initialize ()
	{
		addState(new MainMenuStateMain(this));
		addState(new MainMenuStateMapEditor(this));
		addState(new MainMenuStateLoadMap(this));
		addState(new MainMenuStateMapEditorNew(this));
		addState(new MainMenuStateTrainANN(this));
		addState(new MainMenuStateLoadPop(this));
		addState(new MainMenuStateSelectGenome(this));
	}
}
