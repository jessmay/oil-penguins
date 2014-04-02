using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TempMainMenuStateLoadPop : GUIState {

	public TempMainMenuStateLoadPop (TempMainMenuFSM fsm) : base(fsm) { }
	
	public override string getName () {
		return "Temporary Main Menu Load population";
	}
	
	public override void updateGraphics () {}


	public override void displayGraphics () {

		string path = Options.GADirectory;

		string fileName = getFileFromDirectory(path, "Select a population", "Back", 4);

		if(fileName == null) {
			finiteStateMachine.changeState(typeof(TempMainMenuStateTrainANN));
		}
		else if (!fileName.Equals("")) {
			Options.populationName = fileName;

			finiteStateMachine.changeState(typeof(TempMainMenuStateLoadMap), TempMainMenuStateLoadMap.FROM_HUMAN_TESTS);
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
