using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	public GameObject Wall;
	public GameObject Agent;

	private int width = 20;
	private int height = 20;

	private int gridWidth = 10;
	private int gridHeight = 10;


	public Grid grid;

	// Use this for initialization
	void Start () {

		grid = new Grid (gridWidth, gridHeight, renderer.bounds);

		float xSize = renderer.bounds.size.x/width;
		float ySize = renderer.bounds.size.y/height;

		float wallX = xSize/(Wall.renderer.bounds.size.x/Wall.transform.localScale.x);
		float wallY = ySize/(Wall.renderer.bounds.size.y/Wall.transform.localScale.y);

		Wall.transform.localScale = new Vector3(wallX, wallY, 1);

		Vector3 c = renderer.bounds.center;

		for (int i = -height/2; i <= height/2; ++i) {
			Instantiate (Wall, new Vector3(c.x+i*xSize, c.y-width/2*ySize, c.z), Wall.transform.rotation);
			
			Instantiate (Wall, new Vector3(c.x+i*xSize, c.y+width/2*ySize, c.z), Wall.transform.rotation);
		}

		for (int j = -width/2 +1; j < width/2; ++j) {
			Instantiate (Wall, new Vector3(c.x-height/2*xSize, c.y+j*ySize, c.z), Wall.transform.rotation);

			Instantiate (Wall, new Vector3(c.x+height/2*xSize, c.y+j*ySize, c.z), Wall.transform.rotation);
		}


	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;
			Instantiate(Wall, pos, Wall.transform.rotation);
		}
		if (Input.GetMouseButtonDown (1)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;
			GameObject a = Instantiate(Agent, pos, Wall.transform.rotation) as GameObject;
			grid.add(a);
		}
	}


}
