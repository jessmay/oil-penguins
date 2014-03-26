using UnityEngine;
using System;
using System.Collections;

public abstract class PauseMenu : MonoBehaviour {

	//The GUI skin used to display all graphics and menus for the pause menu
	public GUISkin skin;
	public Color skinNormalColor;
	public Color skinHighlightColor;

	[HideInInspector]
	public GUIStyle button;

	[HideInInspector]
	public GUIStyle box;

	[HideInInspector]
	public GUIStyle label;

	//Max width and height that should be used for any pause interface
	// Any values greater will be cut off on lower resolutions
	[HideInInspector]
	public const int sWidth = 600;
	[HideInInspector]
	public const int sHeight = 440;

	[HideInInspector]
	public const int buttonHeight = 75;

	[HideInInspector]
	public const int width = 200;
	[HideInInspector]
	public const int height = 50;


	
	//Holds if the game is currently paused or not.
	protected bool paused = false;


	//Initialize the pause menu
	protected abstract GUIFSM createFSM();
	protected abstract Type initialPause();

	//The finite state machine for this pause menu.
	public GUIFSM finiteStateMachine {get; private set;}
	
	//Called when entering pause
	//Optional menu argument to move into a specific menu.
	protected abstract void startPause();
	
	//Called when exiting pause
	protected abstract void exitPause();

	protected abstract bool forcedPause();

	//Pause or unpause the game
	private void TogglePause() {
		
		// Unpause
		if(paused) {

			//Unpause if allowed.
			if(!forcedPause())
				unPause();
		}
		
		// Pause
		else {
			pause();
		}
	}


	//Pause the game and enter the given menu state
	public void pause(Type menuState = null, int code = GUIState.DEFAULT_CODE) {

		paused = true;
		
		Time.timeScale = 0.0f;
		
		startPause();

		finiteStateMachine.changeState(menuState, code);
	}


	//Unpause the game
	public void unPause() {

		paused = false;

		Time.timeScale = 1.0f;
		
		exitPause();

		//finiteStateMachine.changeState(getExitState());
	}


	void Awake() {
		
		button = new GUIStyle(skin.button);
		button.fontSize = 25;
		
		box = new GUIStyle(skin.box);
		label = new GUIStyle(skin.label);
		
		setSkinTextures();
		
		createBackgroundBox(new Color(0,0,0,0.4f));

		finiteStateMachine = createFSM();
	}


	//Used for initializing
	void Start() {

		finiteStateMachine.initialize();

		Type initialPauseState = initialPause();

		if(initialPauseState != null) {
			pause(initialPauseState);
		}
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape)) {
			TogglePause();
		}

		//Update current menu if game is paused
		if(paused) {
			finiteStateMachine.update();
		}
		
	}
	
	void OnGUI() {
		
		//Draw current menu if game is paused
		if(paused) {

			//Draw white box over screen
			drawBackgroundBox();

			//Draw pause menu
			finiteStateMachine.display();
		}
	}


	//Create box to fill background with color during pause.
	private void createBackgroundBox(Color color) {

		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0,0, color);
		texture.Apply();
		
		backgroundBox = new GUIStyle();
		backgroundBox.normal.background = texture;
	}


	//Box used to draw a color over the background while game is paused.
	private GUIStyle backgroundBox;
	private void drawBackgroundBox() {
		GUI.Box(new Rect(0,0,Screen.width, Screen.height), GUIContent.none, backgroundBox);
	}

	
	//Pause game if application loses focus
	void OnApplicationFocus(bool focused) {
		
		if ((!paused && !focused)) {
			pause();
		}
	}
	
	public bool isPaused () {
		return paused;
	}


	private void setSkinTextures() {
		Texture2D defaultTexture = createDefaultGUITexture(30, 30, 10);
		Texture2D normalTexture = colorizeTexture(defaultTexture, skinNormalColor);
		Texture2D highlightTexture = colorizeTexture(defaultTexture, skinHighlightColor);

		button.normal.background = normalTexture;
		button.hover.background = highlightTexture;

		box.normal.background = normalTexture;
	}

	private static Texture2D createDefaultGUITexture(int width, int height, int borderSize) {

		Texture2D texture = new Texture2D(width, height);

		for (int x = 0; x < width; ++x) {
			for (int y = 0; y < height; ++y) {

				Color color;
				if (x < borderSize || x >= width - borderSize || y < borderSize || y >= height - borderSize){
					color = Color.black;
				}
				else {
					color = Color.white;
				}

				texture.SetPixel(x, y, color);
			}
		}
		texture.Apply();
		texture.filterMode = FilterMode.Point;

		return texture;
	}

	private static Texture2D colorizeTexture(Texture2D originalTexture, Color color) {

		int width = originalTexture.width;
		int height = originalTexture.height;
		Texture2D colorizedTexture = new Texture2D(width, height);

		for (int x = 0; x < width; ++x) {
			for (int y = 0; y < height; ++y) {
				colorizedTexture.SetPixel(x, y, originalTexture.GetPixel(x,y) * color);
			}
		}

		colorizedTexture.Apply();
		colorizedTexture.filterMode = originalTexture.filterMode;

		return colorizedTexture;
	}
}
