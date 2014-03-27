/*
Jessica May
Joshua Linge
EnvironmentManager.cs

Updated by Joshua Linge on 2014-03-17
 */

using UnityEngine;
using System.IO;
using System.Collections;

public class AITrainingEnvironmentManager : MonoBehaviour {

	public GameObject Wall;
	public GameObject TestSubject;

	public GameObject player;

	private GameObject currPlayer;

	private PauseMenuOld pauseMenu;

	private int mapWidth = 20;
	private int mapHeight = 20;

	private int gridWidth = 10;
	private int gridHeight = 10;


	public Grid grid;
	public Map map;

	// Use this for initialization
	void Awake () {

		//grid = new Grid (gridWidth, gridHeight, renderer.bounds);
	}

	void Start() {

		pauseMenu = GetComponent<PauseMenuOld>();
		pauseMenu.TogglePause();

		//(Instantiate(player, Vector3.zero, player.transform.rotation) as GameObject ).GetComponent<Agent>().grid = grid;
	}

	//Create and initialize the testAgent
	public void createPlayer () {
		
		if (currPlayer != null) {
			Destroy(currPlayer);
		}

		Vector3 startLocation = Vector3.zero;
		currPlayer = Instantiate(TestSubject, startLocation, TestSubject.transform.rotation) as GameObject;
		currPlayer.name = "TestSubject";

		Agent agentScript = currPlayer.GetComponent<Agent>();
		agentScript.grid = grid;
		agentScript.map = map;
	}

	private void createMap(Map newMap) {

		if(map != null)
			map.Dispose();

		map = newMap;

		Bounds mapBounds = map.getBounds();

		if(grid != null)
			grid.Dispose();

		grid = new Grid (gridWidth, gridHeight, mapBounds);
		
		transform.localScale = new Vector3(mapBounds.size.x/renderer.bounds.size.x*transform.localScale.x, mapBounds.size.y/renderer.bounds.size.y*transform.localScale.y, transform.localScale.z);
	}


	//Create the default map
	public void createDefaultMap() {

		createMap (new Map("Default", Wall, mapWidth, mapHeight));

		createPlayer();
	}


	//Load a map from the given file name.
	public void loadMap (string mapName) {

		//Reload the same map?
		//if(map != null && map.name.Equals(mapName))
		//	return;

		Debug.Log("Loading Map " +mapName);

		byte[] bytes = File.ReadAllBytes(Application.dataPath + "/../Maps/"+ mapName +".png");
		Texture2D mapImage = new Texture2D(1,1);
		mapImage.LoadImage(bytes);
		
		createMap(new Map(mapName, Wall, mapImage));

		createPlayer();
	}


	//Save the current map to the given file name.
	public void saveMap (string mapName) {
		
		Debug.Log("Saving Map " +mapName);

		if(!Directory.Exists(Application.dataPath + "/../Maps")) 
			Directory.CreateDirectory(Application.dataPath + "/../Maps");
		
		byte[] bytes = map.saveMap().EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../Maps/" + mapName +".png", bytes);

	}


	//Save the genetic algorithm population to the given file name.
	public void saveGA (string fileName) {

		string path = Application.dataPath + "/../GA";
		if(!Directory.Exists(path)) 
			Directory.CreateDirectory(path);

		TestAgent ta = currPlayer.GetComponent<TestAgent>();
		
		if (ta == null)
			return;

		if (File.Exists(path +"/" +fileName+ ".txt")) {

			int count = 1;
			while (File.Exists(path +"/" +fileName +"_"+ count + ".txt"))
				++count;

			fileName += "_"+count;
		}

		Debug.Log("Saving population to file "+fileName +".");

		StreamWriter sr = File.CreateText(path +"/" +fileName + ".txt");

		GeneticAlgorithm ga = ta.geneticAlgorithm;

		sr.WriteLine(ga.getPopulationAsAString());

		sr.Close();

		Debug.Log("Population has been saved.");
	}


	//Load a genetic algorithm population from the given file name.
	public void loadGA (string fileName) {

		Debug.Log ("Loading Population from file "+fileName +".");

		StreamReader reader = new StreamReader(Application.dataPath + "/../GA/"+ fileName +".txt");

		string fileContents = reader.ReadToEnd();
		reader.Close();
		reader.Dispose();

		GeneticAlgorithm ga = GeneticAlgorithm.loadPopulationFromString(fileContents);

		TestAgent ta = currPlayer.GetComponent<TestAgent>();
		ta.setGeneticAlgorithm(ga);
		ta.reset();

		Debug.Log("Population has been loaded.");
	}
	

	// Update is called once per frame
	void FixedUpdate () {

		//Place/remove wall at the given mouse location
		if (Input.GetMouseButtonDown (0)) {
			Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;

			//If wall already exists at the location, remove it
			if(!map.addWall(map.getCellIndex(pos)))
				map.removeWall(map.getCellIndex(pos));
		}

		/*
		//Place agent at the given mouse location
		if (Input.GetMouseButtonDown (1)) {
			Vector3 pos = DebugRenderer.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;
			GameObject a = Instantiate(Agent, pos, Agent.transform.rotation) as GameObject;

			Agent agentScript = a.GetComponent<Agent>();
			agentScript.background = this;

			grid.add(agentScript);
		}
		*/
	}


	//Display menu to the right side of the screen
	Vector2 scrollPosition = Vector2.zero;
	void OnGUI () {

		if(pauseMenu.isPaused())
			return;

		int width = 200;
		int height = 50;
		int scrollBarWidth = 16;


		int numButtons = 12;
		int currButton = 0;
		scrollPosition = GUI.BeginScrollView (new Rect (Screen.width - width , 0, width, Screen.height), scrollPosition, new Rect (0, 0, width-scrollBarWidth, height * numButtons));

		//Set time scale back to 1
		if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), "1")) {
			Time.timeScale = pauseMenu.timeScale = 1;
		}

		++currButton;

		//Set time scale to 10 to speed up run
		if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), "10")) {
			Time.timeScale = pauseMenu.timeScale = 10;
		}

		++currButton;

		//Set time scale to 20 to speed up run
		if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), "20")) {
			Time.timeScale = pauseMenu.timeScale = 20;
		}

		++currButton;

		//Set time scale to 50 to speed up run
		if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), "50")) {
			Time.timeScale = pauseMenu.timeScale = 50;
		}
		
		++currButton;

		//Do not display if player is null
		if (currPlayer != null) {

			//Get script for the testAgent
			TestAgent ta = currPlayer.GetComponent<TestAgent>();
			
			GUI.enabled = !ta.testing && !ta.mouseIsTarget;

			//Change to the next target for the agent to seek.
			if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), "Change target")) {
				ta.moveToNextTarget();
				ta.reset();
			}

			++currButton;

			GUI.enabled = true;

			//Stop or resume genetic algorithm testing.
			if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), (ta.testing?"Stop":"Resume")+" testing")) {
				ta.toggleTesting(pauseMenu.timeScale);
			}
			
			++currButton;

			//Save the current population to file.
			if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), "Save population")) {
				pauseMenu.TogglePause("SaveGA");
			}
			
			++currButton;

			//Load a new population from a file.
			if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), "Load population")) {
				pauseMenu.TogglePause("LoadGA");
			}
			
			++currButton;

			//Enable or disable seeking targets.
			if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), (!ta.targetsEnabled?"Enable":"Disable") +" Targets")) {
				ta.targetsEnabled = !ta.targetsEnabled;
			}
			
			++currButton;

			GUI.enabled = ta.targetsEnabled && !ta.mouseIsTarget;

			//Use nine or four targets in each set.
			bool T4 = ta.totalTargets() == 4;
			if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), T4?"Nine Targets":"Four Targets")) {
				
				if(T4) {
					ta.createNineTargets();
				}
				else {
					ta.createFourTargets();
				}
			}

			GUI.enabled = !ta.testing && ta.targetsEnabled;
			
			++currButton;

			//Use the mouse position as the target for the agent to seek.
			if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), "Use mouse as Target")) {
				ta.mouseIsTarget = !ta.mouseIsTarget;
			}

			++currButton;

			GUI.enabled = !ta.testing;

			//Stop the Neural Network from acting and allow the user to control the agent.
			if (GUI.Button(new Rect(0, height*currButton, width-scrollBarWidth, height), !ta.userControl?"Take control": "Give control")) {
				ta.userControl = !ta.userControl;
			}

			GUI.enabled = true;

		}

		GUI.EndScrollView ();
	}
}
