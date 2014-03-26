using UnityEngine;
using System;
using System.Collections;

public abstract class GUIState {

	//Variables to make things easier to reach.
	protected GUIStyle button {get; private set;}
	protected GUIStyle box {get; private set;}
	protected GUIStyle label {get; private set;}
	
	protected int sWidth {get; private set;}
	protected int sHeight {get; private set;}
	
	protected int width {get; private set;}
	protected int height {get; private set;}
	protected int buttonHeight {get; private set;}

	protected GUIFSM finiteStateMachine;
	
	public abstract string getName();
	public abstract void enter();
	public abstract void exit();
	public abstract void updateGraphics();
	public abstract void displayGraphics();

	public GUIState (GUIFSM fsm) {

		finiteStateMachine = fsm;

		button = fsm.pauseMenu.button;
		box = fsm.pauseMenu.box;
		label = fsm.pauseMenu.label;
		
		height = PauseMenu.height;
		width = PauseMenu.width;

		sWidth = PauseMenu.sWidth;
		sHeight = PauseMenu.sHeight;
		buttonHeight = PauseMenu.buttonHeight;
	}

	//Used to give states extra information
	//States within states.
	public int statusCode {get; private set;}
	public const int DEFAULT_CODE = 0;

	protected abstract bool isValidStatus(int statusCode);

	public void setStatusCode(int code) {

		if(!isValidStatus(code)) {

			throw new Exception("Invalid status of "+ code +" when entering into " +GetType());
		}

		statusCode = code;
	}


	//Generic menu items:

	//Display confirmation box with the given message
	//Return 1 if user clicks yes
	//		-1 if user clicks no
	//		 0 if nothing is clicked
	protected int confirmationBox(string message, string yesMessage = "Yes", string noMessage = "No") {

		label.fontSize = 30;

		int labelHeight = Mathf.CeilToInt(label.CalcHeight(new GUIContent(message), sWidth/2));

		int labelBorderSize = 25;

		int boxSize = labelBorderSize*2 + labelHeight + buttonHeight;


		//Background box
		GUI.Box (new Rect(Screen.width/2 - sWidth*3/10, Screen.height/2 - boxSize/2, sWidth*3/5, boxSize), GUIContent.none, box);
		
		//Display message
		GUI.Label(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + (boxSize/2 - buttonHeight) - (labelHeight + labelBorderSize), sWidth/2, labelHeight), message, label);
		
		//Buttons
		button.fontSize = 25;
		
		// User pressed yes
		if(GUI.Button(new Rect(Screen.width/2 - sWidth*3/10, Screen.height/2 + boxSize/2 - buttonHeight, sWidth*3/10+ button.border.left/2, buttonHeight), yesMessage, button)) {
			
			return 1;
		}
		
		// User pressed no
		if(GUI.Button(new Rect(Screen.width/2 - button.border.right/2, Screen.height/2 + boxSize/2 - buttonHeight, sWidth*3/10 + button.border.right/2, buttonHeight), noMessage, button)) {
			
			return -1;
		}
		
		return 0;
	}
}
