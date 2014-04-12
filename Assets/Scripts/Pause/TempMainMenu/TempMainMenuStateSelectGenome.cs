using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class TempMainMenuStateSelectGenome : GUIState {
	
	public TempMainMenuStateSelectGenome (TempMainMenuFSM fsm) : base(fsm) { }
	
	public override string getName () {
		return "Temporary Main Menu Select Genome";
	}
	
	public override void updateGraphics () {}
	
	
	public override void displayGraphics () {
		
		Type[] types = typeof(Genome).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Genome))).ToArray();

		string[] genomeNames = new string[types.Length];
		
		for (int currType = 0; currType < types.Length; ++currType){
			genomeNames[currType] = types[currType].Name;
		}

		string genomeType = getItemFromList(genomeNames, "Select a Genome", "Back", 4);
		
		if(genomeType == null) {
			finiteStateMachine.changeState(typeof(TempMainMenuStateTrainANN));
		}
		else if (!genomeType.Equals("")) {
			Options.genomeType = genomeType;
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

