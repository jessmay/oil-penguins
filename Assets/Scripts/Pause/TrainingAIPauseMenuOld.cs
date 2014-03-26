/*
Joshua Linge
MainPauseMenu.cs

2014-03-17
*/

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TrainingAIPauseMenuOld : PauseMenuOld {
	
	public enum MenuItem {Main, LoadMap, SaveMap, LoadGA, SaveGA}
	private MenuItem currentMenu;
	private Dictionary<string, MenuItem> dictionary;

	private AITrainingEnvironmentManager background;
	
	private int width = 200;
	private int height = 50;
	

	//Initialize
	public TrainingAIPauseMenuOld() {
		
		dictionary = new Dictionary<string, MenuItem>();
		
		foreach (var value in Enum.GetValues(typeof(MenuItem))) {
			dictionary.Add(((MenuItem)value).ToString(), (MenuItem)value);
		}
	}


	//Nothing to update
	public override void updateMenu () {}


	//Initialize
	public override void setUp() {

		background = GetComponent<AITrainingEnvironmentManager>();
	}


	//Display the current menu.
	public override void displayMenu() {

		switch (currentMenu) {
			case MenuItem.SaveGA:
				displaySaveGAMenu();
				break;
			case MenuItem.LoadGA:
				displayLoadGAMenu();
				break;
			case MenuItem.SaveMap:
				displaySaveMapMenu();
				break;
			case MenuItem.LoadMap:
				displayLoadMapMenu();
				break;
			case MenuItem.Main:
			default:
				displayMainMenu();
				break;
		}

	}


	//Display main menu to the user.
	private void displayMainMenu() {
		
		GUI.Box(new Rect(Screen.width/2 - width/2, Screen.height/2 - 2*height, width, height),GUIContent.none);
		
		GUIStyle gs = new GUIStyle(GUI.skin.label);
		gs.alignment = TextAnchor.MiddleCenter;
		gs.fontSize = 20;
		
		GUI.color = Color.white;
		GUI.Label(new Rect(Screen.width/2 - width/2, Screen.height/2 - 2*height, width, height),"Paused", gs);

		// Unpause Button
		if(GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height/2 - height, width, height),(background.map == null?"Start New":"Resume"))) {

			//If no map has been loaded, create the default map.
			if(background.map == null) {
				
				background.createDefaultMap();
			}
			TogglePause();
		}

		//Load a map.
		if(GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height/2, width, height),"Load Map")) {
			
			currentMenu = MenuItem.LoadMap;
		}

		GUI.enabled = (background.map != null);

		//Save current map.
		if(GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height/2 + height, width, height),"Save Map")) {

			currentMenu = MenuItem.SaveMap;
		}

		GUI.enabled = true;
	}


	//Display a list of genetic algorithms to the user.
	//If one is selected, load it.
	private void displayLoadGAMenu() {
		string name = getFileFromDirectory(Application.dataPath + "/../GA","Pick a GA:","Back");
		
		if (name == null) {
			TogglePause();
		}
		else if (!name.Equals("")) {
			background.loadGA(name);
			TogglePause();
		}
	}


	//Display a list of maps to the user.
	//If one is selected, load it.
	private void displayLoadMapMenu() {

		string name = getFileFromDirectory(Application.dataPath + "/../Maps","Pick a map:","Back");

		if (name == null) {
			currentMenu = MenuItem.Main;
		}
		else if (!name.Equals("")) {
			background.loadMap(name);
			TogglePause();
		}
	}


	//Displays an interface that allows the user to select a file from a directory.
	//Returns 	"" if no file is selected,
	//			null if the user clicks the exit button
	//			a filename if the user clicks on a file displayed
	Vector2 scrollPosition = Vector2.zero;
	private string getFileFromDirectory(string path, string message, string exit) {

		//If the path does not exist, create it.
		if(!Directory.Exists(path)) 
			Directory.CreateDirectory(path);

		//Get all files in the current directory.
		string[] fileList = Directory.GetFiles(path + "/");
		List<string> fileNames = new List<string>();

		//Loop through each file in the directory
		for (int currFile = 0; currFile < fileList.Length; ++currFile) {

			//Skip all hidden files (ex. .DS_Store)
			if(fileList[currFile].Remove(0,fileList[currFile].LastIndexOf('/')+1)[0] == '.')
				continue;

			//Add file name to the list of file names
			//Path and directory information is removed first.
			fileNames.Add(fileList[currFile].Remove(fileList[currFile].LastIndexOf('.')).Remove(0,fileList[currFile].LastIndexOf('/')+1));
		}
		
		fileList = fileNames.ToArray();


		//Gui style to display label.
		GUIStyle gs = new GUIStyle(GUI.skin.label);
		gs.alignment = TextAnchor.MiddleCenter;
		gs.fontSize = 20;

		//Menu title
		GUI.Box(new Rect(Screen.width/2 - width/2, Screen.height/2 - 3*height, width, height),GUIContent.none);
		GUI.Label(new Rect(Screen.width/2 - width/2, Screen.height/2 - 3*height, width, height),message, gs);
		
		
		//SrollView background
		GUI.Box(new Rect (Screen.width/2 - width/2 ,Screen.height/2 - 2*height, width, 4*height),GUIContent.none);

		int scrollBarWidth = 16;
		int maxFilesPerScroll = 4;

		//Scrollable view to display all file options to the user.
		scrollPosition = GUI.BeginScrollView (new Rect (Screen.width/2 - width/2 ,Screen.height/2 - 2*height, width, maxFilesPerScroll*height), scrollPosition, new Rect (0, 0, width-scrollBarWidth, height * fileList.Length));

		//Button for each file in the directory
		for (int currMap = 0; currMap < fileList.Length; ++currMap) {

			//Return filename if button is clicked.
			if(GUI.Button (new Rect(0, (0+currMap)*height, width-(fileList.Length> maxFilesPerScroll?scrollBarWidth:0), height), fileList[currMap])) {

				return fileList[currMap];
			}
		}

		// End the scroll view that we began above.
		GUI.EndScrollView ();

		//Exit or back button
		if(GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height/2 + 2*height, width, height),exit)) {

			scrollPosition = Vector2.zero;
			return null;
		}

		//No action taken, return.
		return "";
	}


	//Get a map name from the user and then save the current map to file.
	private void displaySaveMapMenu() {

		string name = getInput("Map name:", "Save", "Back");

		if (name == null) {
			currentMenu = MenuItem.Main;
		}
		else if (!name.Equals("")) {
			background.saveMap(name);
			currentMenu = MenuItem.Main;
		}
	}


	//Get a genetic algorithm name from the user and then save the current population to file.
	private void displaySaveGAMenu() {

		string name = getInput("GA name:", "Save", "Back");
		
		if (name == null) {
			TogglePause();
		}
		else if (!name.Equals("")) {
			background.saveGA(name);
			TogglePause();
		}
	}


	//Displays an interface that allows the user to input a string.
	//Returns 	"" while the user is edition the text
	//			null if the user clicks the exit button
	//			a string if the user accepts the typed input
	string fileName = "";
	public string getInput(string message, string accept, string exit) {
		
		GUI.Box(new Rect(Screen.width/2 - width/2, Screen.height/2 - 2*height, width, height),GUIContent.none);
		
		GUIStyle gs = new GUIStyle(GUI.skin.label);
		gs.alignment = TextAnchor.MiddleCenter;
		gs.fontSize = 20;
		
		GUI.color = Color.white;
		GUI.Label(new Rect(Screen.width/2 - width/2, Screen.height/2 - 2*height, width, height),message, gs);
		
		
		//Get text
		fileName = GUI.TextField (new Rect(Screen.width/2 - width/2, Screen.height/2 - height, width, height), fileName, 50);
		
		
		if(GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height/2, width, height),accept) && !fileName.Equals("")) {
			
			string temp = fileName;
			fileName = "";
			return temp;
		}
		
		if(GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height/2 + height, width, height),exit)) {

			fileName = "";
			return null;
		}
		return "";
	}


	//Load the given menu item
	//Main menu is loaded by default if no menu is given.
	public override void startPause(string menu = null) {
		
		if (menu == null || !dictionary.ContainsKey(menu))
			currentMenu = MenuItem.Main;
		else
			currentMenu = dictionary[menu];
	}


	//Create the default map before exiting pause if no map has been loaded.
	public override void exitPause() {

		if(background.map == null) {
			
			background.createDefaultMap();
		}
	}
}
