using UnityEngine;
using System.Collections;

public class MainMenuStateMain : MainMenuState {

	public MainMenuStateMain (MainMenuFSM fsm) : base(fsm) { }

	public override string getName () {
		return "Main Menu State";
	}

	public override void updateGraphics () {}

	public override void displayGraphics () {
		base.displayGraphics();

		label.fontSize = 30;
		
		//GUI.Label(new Rect(Screen.width/2 - width, Screen.height/2 - buttonHeight*3/2, width*2, buttonHeight),"Intellectual Penguins", label); //Change to logo
		
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - 2 * (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Play", button)) {

			finiteStateMachine.changeState(typeof(MainMenuStateLoadMap), MainMenuStateLoadMap.FROM_PLAY_GAME);
		}

		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - 1 * (buttonHeight - button.border.top), sWidth/2, buttonHeight),"High Scores", button)) {

			finiteStateMachine.changeState(typeof(MainMenuStateHighScores));
		}

		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + 0 * (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Map Editor", button)) {

			finiteStateMachine.changeState(typeof(MainMenuStateMapEditor));
		}

		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + 1 * (buttonHeight - button.border.top), sWidth/2, buttonHeight),"Train ANN", button)) {

			finiteStateMachine.changeState(typeof(MainMenuStateTrainANN));
		}

	}

	public override void enter () {
		Options.mapName = null;
		Options.Testing = false;
		Options.play = false;

		Options.populationName = null;
	}

	public override void exit () {}

	protected override bool isValidStatus (int statusCode) {
		return (statusCode == DEFAULT_CODE);
	}
}
