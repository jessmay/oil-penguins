/*
Joshua Linge
PauseMenu.cs

Edited from SEE project

2014-03-17
*/

using UnityEngine;
using System.Collections;

public abstract class PauseMenu : MonoBehaviour {

	//Holds if the game is currently paused or not.
	protected bool paused = false;

	//Called to update the menu.
	public abstract void updateMenu();
	
	//Called to display the menu
	public abstract void displayMenu();
	
	//Called to setup the pause menu
	public abstract void setUp();
	
	//Called when entering pause
	//Optional menu argument to move into a specific menu.
	public abstract void startPause(string menu);
	
	//Called when exiting pause
	public abstract void exitPause();

	public float timeScale = 50;

	//Pause or unpause the game
	public bool TogglePause(string menu = null) {
		
		// Unpause
		if(Time.timeScale == 0.0f) {
			
			Time.timeScale = timeScale;
		}
		
		// Pause
		else {
			
			Time.timeScale = 0.0f;
		}
		
		paused = (Time.timeScale == 0.0f);
		
		if(paused)
			startPause(menu);
		else
			exitPause();
		
		return paused;
	}


	//Initialize
	void Start() {

		//Setup pause menu
		setUp();
	}

	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			TogglePause();
		}

		//Update current menu if game is paused
		if(paused) {
			updateMenu();
		}
	}
	
	void OnGUI() {
		
		//Draw current menu if game is paused
		if(paused) {
			//Draw black box over screen
			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0,0, new Color(0,0,0,0.4f));
			texture.Apply();

			GUIStyle box = new GUIStyle();
			box.normal.background = texture;
			GUI.Box(new Rect(0,0,Screen.width, Screen.height), GUIContent.none, box);

			//Draw pause menu
			displayMenu();
		}
	}

	public bool isPaused () {
		return paused;
	}

}
