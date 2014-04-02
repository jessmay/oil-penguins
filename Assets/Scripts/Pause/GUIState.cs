using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

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


	//Displays an interface that allows the user to select a file from a directory.
	//Returns 	"" if no file is selected,
	//			null if the user clicks the exit button
	//			a filename if the user clicks on a file displayed
	protected Vector2 scrollPosition;
	protected string getFileFromDirectory(string path, string message, string exit, int maxFilesPerScroll = 4) {
		
		//If the path does not exist, create it.
		if(!Directory.Exists(path)) 
			Directory.CreateDirectory(path);
		
		//Get all files in the current directory.
		string[] fileList = Directory.GetFiles(path + "/");
		List<string> fileNames = new List<string>();
		
		//Loop through each file in the directory
		for (int currFile = 0; currFile < fileList.Length; ++currFile) {
			
			//Skip all hidden files (example: .DS_Store)
			if(fileList[currFile].Remove(0,fileList[currFile].LastIndexOf('/')+1)[0] == '.')
				continue;
			
			//Add file name to the list of file names
			//Path and directory information is removed first.
			fileNames.Add(fileList[currFile].Remove(fileList[currFile].LastIndexOf('.')).Remove(0,fileList[currFile].LastIndexOf('/')+1));
		}
		
		fileList = fileNames.ToArray();
		
		
		label.fontSize = 30;
		
		int scrollBarWidth = 16;

		//Menu title
		GUI.Box(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - (maxFilesPerScroll + 2)*(buttonHeight- button.border.top)/2, sWidth/2, buttonHeight),GUIContent.none, box);
		GUI.Label(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - maxFilesPerScroll*(buttonHeight- button.border.top)/2 - (buttonHeight- button.border.top) + 20, sWidth/2, buttonHeight), message, label);
		
		//SrollView background
		GUI.Box(new Rect (Screen.width/2 - sWidth/4 ,Screen.height/2 - maxFilesPerScroll*(buttonHeight - button.border.top)/2, sWidth/2, (buttonHeight* maxFilesPerScroll) - (button.border.top* (maxFilesPerScroll-1))),GUIContent.none, box);
		
		
		//Scrollable view to display all file options to the user.
		scrollPosition = GUI.BeginScrollView (new Rect (Screen.width/2 - sWidth/4 ,Screen.height/2 - maxFilesPerScroll*(buttonHeight- button.border.top)/2 + button.border.top, sWidth/2, (buttonHeight* maxFilesPerScroll) - (button.border.top* (maxFilesPerScroll-1)) - 2*button.border.top), scrollPosition, new Rect (0, 0, sWidth/2-scrollBarWidth, (buttonHeight* fileList.Length) - (button.border.top* (fileList.Length-1))- 2*button.border.top ));
		
		//Button for each file in the directory
		for (int currFile = 0; currFile < fileList.Length; ++currFile) {
			
			//Return filename if button is clicked.
			if(GUI.Button (new Rect(0, currFile*(buttonHeight-button.border.top) -button.border.top, sWidth/2-(fileList.Length> maxFilesPerScroll?scrollBarWidth:0), buttonHeight), fileList[currFile], button)) {
				
				return fileList[currFile];
			}
		}
		
		// End the scroll view that we began above.
		GUI.EndScrollView ();
		
		//Exit or back button
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + maxFilesPerScroll*(buttonHeight- button.border.top)/2, sWidth/2, buttonHeight), exit, button)) {
			
			return null;
		}
		
		return "";
	}
}
