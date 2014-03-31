using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TempMainMenuStateLoad : GUIState {

	public static int FROM_MAP_EDITOR = 1;
	public static int FROM_HUMAN_TESTS = 2;

	public TempMainMenuStateLoad (TempMainMenuFSM fsm) : base(fsm) { }
	
	public override string getName () {
		return "Temporary Main Menu Map Editor Load";
	}
	
	public override void updateGraphics () {}


	Vector2 scrollPosition;

	public override void displayGraphics () {

		string path = Options.MapDirectory;
		
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

		//Menu title
		GUI.Box(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - 3*(buttonHeight- button.border.top), sWidth/2, buttonHeight),GUIContent.none, box);
		GUI.Label(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 - 3*(buttonHeight- button.border.top) + 20, sWidth/2, buttonHeight), "Select a map", label);
		
		
		//SrollView background
		GUI.Box(new Rect (Screen.width/2 - sWidth/4 ,Screen.height/2 - 2*(buttonHeight - button.border.top), sWidth/2, 4*buttonHeight),GUIContent.none, box);
		
		int scrollBarWidth = 16;
		int maxFilesPerScroll = 4;
		
		//Scrollable view to display all file options to the user.
		scrollPosition = GUI.BeginScrollView (new Rect (Screen.width/2 - sWidth/4 ,Screen.height/2 - 2*(buttonHeight- button.border.top), sWidth/2, maxFilesPerScroll*buttonHeight), scrollPosition, new Rect (0, 0, sWidth/2-scrollBarWidth, buttonHeight * fileList.Length));
		
		//Button for each file in the directory
		for (int currMap = 0; currMap < fileList.Length; ++currMap) {
			
			//Return filename if button is clicked.
			if(GUI.Button (new Rect(0, (0+currMap)*(buttonHeight - button.border.top), sWidth/2-(fileList.Length> maxFilesPerScroll?scrollBarWidth:0), buttonHeight), fileList[currMap], button)) {

				Options.mapName = fileList[currMap];

				if(statusCode == FROM_MAP_EDITOR)
					Application.LoadLevel("MapEditor");
				else if(statusCode == FROM_HUMAN_TESTS)
					Application.LoadLevel("TestScene");

				finiteStateMachine.pauseMenu.unPause();
				//return fileList[currMap];
			}
		}
		
		// End the scroll view that we began above.
		GUI.EndScrollView ();
		
		//Exit or back button
		if(GUI.Button(new Rect(Screen.width/2 - sWidth/4, Screen.height/2 + 2*(buttonHeight- button.border.top), sWidth/2, buttonHeight), "Back", button)) {

			finiteStateMachine.changeToPreviousState();
			//finiteStateMachine.changeState(typeof(TempMainMenuStateMapEditor));
			//return null;
		}

		//return "";
	}
		
	public override void enter () {
		scrollPosition = Vector2.zero;
	}
	
	public override void exit () {}
	
	protected override bool isValidStatus (int statusCode) {
		return (statusCode == FROM_MAP_EDITOR || statusCode == FROM_HUMAN_TESTS);
	}
}
