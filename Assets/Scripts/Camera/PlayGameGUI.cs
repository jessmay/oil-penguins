﻿using UnityEngine;
using System.Collections;

public class PlayGameGUI : MonoBehaviour {

	public static int GUISize = 120;

	public GUISkin skin;

	[HideInInspector]
	public GUIStyle button;
	
	[HideInInspector]
	public GUIStyle box;
	
	[HideInInspector]
	public GUIStyle label;

	public GameMap gameMap;
	public WaveManager waveManager;
	private MiniMap miniMap;
	private PauseMenu pauseMenu;

	void Awake() {
		Options.play = true;
	}

	// Use this for initialization
	void Start () {
		
		button = new GUIStyle(skin.button);
		button.fontSize = 25;
		
		box = new GUIStyle(skin.box);
		label = new GUIStyle(skin.label);
		
		PauseMenu.setSkinTextures(button, box);

		miniMap = GetComponent<MiniMap>();
		pauseMenu = GetComponent<PauseMenu>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

		GUI.Box(new Rect(0, Screen.height - GUISize, Screen.width, GUISize), GUIContent.none, box);

		int GUIStart = Screen.width/2 - 400;

		button.fontSize = 25;

		label.fontSize = 20;
		label.alignment = TextAnchor.UpperLeft;


		//Wave information
		GUI.Box(new Rect(GUIStart, Screen.height - GUISize , 200, GUISize), GUIContent.none, box);

		if(waveManager.betweenWaves) {
			GUI.Label(new Rect(GUIStart + 20, Screen.height - GUISize + 10, 160, 200), "Wave " +waveManager.waveNumber +" starts in\n" +Mathf.RoundToInt(waveManager.waveStartTime - Time.time) +" seconds.", label);

			button.fontSize = 15;

			GUI.enabled = !pauseMenu.isPaused();

			if(GUI.Button(new Rect(GUIStart + 20 + 20, Screen.height - PauseMenu.buttonHeight/2 - 20, 120, PauseMenu.buttonHeight/2), "Start Wave", button)) {
				waveManager.waveStart();
			}

			GUI.enabled = true;

			button.fontSize = 25;
		}
		else {
			label.alignment = TextAnchor.UpperCenter;

			label.fontSize = 25;

			GUI.Label(new Rect(GUIStart + 20, Screen.height - GUISize + 25, 160, 40), "Wave " + waveManager.waveNumber, label);

			label.fontSize = 20;

			float playTime = Time.time - waveManager.waveStartTime;

			GUI.Label(new Rect(GUIStart + 20, Screen.height - GUISize + 60, 160, 30), string.Format("{0:00}:{1:00}", ((int)playTime)/60, ((int)playTime)%60), label);

			label.alignment = TextAnchor.UpperLeft;
		}

		//Agent information:
		GUI.Box(new Rect(GUIStart + 190, Screen.height - GUISize , 230, GUISize), GUIContent.none, box);

		label.alignment = TextAnchor.MiddleLeft;

		string agentInformation = gameMap.humansKilled + " human" +(gameMap.humansKilled==1?"":"s") +" killed" +"\n"
				+gameMap.HumansOnMap.Count.ToString() +" human" +(gameMap.HumansOnMap.Count==1?"":"s") +" left" +"\n" 
				+gameMap.PenguinsOnMap.Count.ToString() +" penguin" +(gameMap.PenguinsOnMap.Count==1?"":"s") +" on map" +"\n"
				+gameMap.sleepingPenguins.ToString() +" penguin" +(gameMap.sleepingPenguins==1?"":"s") +" sleeping";

		GUI.Label(new Rect(GUIStart + 215, Screen.height - GUISize + 10, 220, GUISize - 20), agentInformation, label);

		label.alignment = TextAnchor.UpperCenter;



		button.fontSize = 20;

		GUI.Box(new Rect(GUIStart + 405, Screen.height - GUISize, 260, GUISize), GUIContent.none, box);

		GUI.Label(new Rect(GUIStart + 405, Screen.height - GUISize + 10, 260, GUISize - 20), "Spawnable Penguins: " + gameMap.numPenguinsSpawnable, label);

		GUI.enabled = !pauseMenu.isPaused() && gameMap.numPenguinsSpawnable > 0;
		if(GUI.Button(new Rect(GUIStart + 435, Screen.height - 50 - 20, PauseMenu.width, 50), "Spawn Penguin", button)) {
			gameMap.spawnPenguin();
		}
		
		GUI.enabled = true;


		GUI.Box(new Rect (GUIStart + 655, Screen.height - GUISize, 800 - 655, GUISize), GUIContent.none, box);

		miniMap.DisplayMiniMap(miniMap.getLocationOnScreen(new Vector2(GUIStart + 800, Screen.height)));

	}
}
