using UnityEngine;
using System.Collections;

public class MapEditorGUI : MonoBehaviour {

	public static int GUISize = 120;

	public GUISkin skin;

	[HideInInspector]
	public GUIStyle button;
	
	[HideInInspector]
	public GUIStyle box;
	
	[HideInInspector]
	public GUIStyle label;

	public GameMap gameMap;
	public MapEditor mapEditor;
	private MiniMap miniMap;
	private PauseMenu pauseMenu;

	Rect[] buttonBounds;

	void Awake() {
		Options.mapEditing = true;
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


		int GUIStart = Screen.width/2 - 400;
		float TextureArea = 665;
		float spaceBetween = TextureArea/(mapEditor.PlaceableItems.Length+1);
		int textureDisplaySize = 50;

		buttonBounds = new Rect[4];
		for(int currItem = 0; currItem < mapEditor.PlaceableItems.Length; ++currItem) {
			buttonBounds[currItem] = new Rect(GUIStart + spaceBetween * (currItem + 1) - textureDisplaySize/2,  Screen.height - GUISize/2 - textureDisplaySize/2, textureDisplaySize, textureDisplaySize);
		}

	}
	
	// Update is called once per frame
	void Update () {
		
		if(pauseMenu.isPaused())
			return;


		//Check to see if item type buttons have been pressed.
		//Update current item if so.
		if(Input.GetMouseButtonDown(0)) {

			Vector3 mousePosition = Input.mousePosition;
			mousePosition.y = Screen.height - mousePosition.y;

			for(int currItem = 0; currItem < mapEditor.PlaceableItems.Length; ++currItem) {
				if(inBounds(buttonBounds[currItem], mousePosition)) {
					mapEditor.itemIndex = currItem;
				}
			}
		}
	}

	private bool inBounds(Rect bounds, Vector3 point) {
		return (point.x >= bounds.xMin && point.x <= bounds.xMax && point.y >= bounds.yMin && point.y <= bounds.yMax);
	}

	void OnGUI() {

		GUI.Box(new Rect(0, Screen.height - GUISize, Screen.width, GUISize), GUIContent.none, box);

		int GUIStart = Screen.width/2 - 400;

		button.fontSize = 25;

		label.fontSize = 20;
		label.alignment = TextAnchor.UpperLeft;


		float TextureArea = 665;

		GUI.Box(new Rect(GUIStart, Screen.height - GUISize, TextureArea, GUISize), GUIContent.none, box);

		float spaceBetween = TextureArea/(mapEditor.PlaceableItems.Length+1);


		DebugRenderer.drawLineRect(new Rect(GUIStart + spaceBetween * (mapEditor.itemIndex + 1) - 75/2,  Screen.height - GUISize/2 - 75/2, 75, 75), 5, Color.white);


		for(int currItem = 0; currItem < mapEditor.PlaceableItems.Length; ++currItem) {
			GUI.DrawTexture(buttonBounds[currItem], mapEditor.PlaceableItems[currItem]);
		}


		GUI.Box(new Rect (GUIStart + 655, Screen.height - GUISize, 800 - 655, GUISize), GUIContent.none, box);

		miniMap.DisplayMiniMap(miniMap.getLocationOnScreen(new Vector2(GUIStart + 800, Screen.height)));

	}
}
