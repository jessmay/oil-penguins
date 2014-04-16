using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MainMenuStateLoadPop : MainMenuState {

	public MainMenuStateLoadPop (MainMenuFSM fsm) : base(fsm) { }
	
	public override string getName () {
		return "Main Menu Load population";
	}
	
	public override void updateGraphics () {}


	public override void displayGraphics () {
		base.displayGraphics();
		
		string path = Options.GADirectory;

		string fileName = getFileFromDirectory(path, "Select a population", "Back", 4);

		if(fileName == null) {
			finiteStateMachine.changeState(typeof(MainMenuStateTrainANN));
		}
		else if (!fileName.Equals("")) {
			Options.populationName = fileName;

			finiteStateMachine.changeState(typeof(MainMenuStateLoadMap), MainMenuStateLoadMap.FROM_HUMAN_TESTS);
		}
	}
		
	public override void enter () {
		scrollPosition = Vector2.zero;
	}
	
	public override void exit () {}
	
	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
