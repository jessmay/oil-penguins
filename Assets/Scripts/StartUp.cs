using UnityEngine;
using System.Collections;

public class StartUp : MonoBehaviour {

	public GameObject Wall;
	private int width = 20;
	private int height = 20;

	// Use this for initialization
	void Start () {

		float xSize = renderer.bounds.size.x/width;
		float ySize = renderer.bounds.size.y/height;

		float wallX = xSize/(Wall.renderer.bounds.size.x/Wall.transform.localScale.x);
		float wallY = ySize/(Wall.renderer.bounds.size.y/Wall.transform.localScale.y);

		Wall.transform.localScale = new Vector3(wallX, wallY, 1);

		Vector3 c = renderer.bounds.center;

		for (int i = -height/2; i <= height/2; ++i) {
			GameObject tempWall = Instantiate (Wall, new Vector3(c.x+i*xSize, c.y-width/2*ySize, c.z), Wall.transform.rotation) as GameObject;
			//tempWall.transform.localScale = new Vector3(wallX, wallY, 1);
			
			tempWall = Instantiate (Wall, new Vector3(c.x+i*xSize, c.y+width/2*ySize, c.z), Wall.transform.rotation) as GameObject;
			//tempWall.transform.localScale = new Vector3(wallX, wallY, 1);

		}

		for (int j = -width/2 +1; j < width/2; ++j) {
			GameObject tempWall = Instantiate (Wall, new Vector3(c.x-height/2*xSize, c.y+j*ySize, c.z), Wall.transform.rotation) as GameObject;
			//tempWall.transform.localScale = new Vector3(wallX, wallY, 1);

			tempWall = Instantiate (Wall, new Vector3(c.x+height/2*xSize, c.y+j*ySize, c.z), Wall.transform.rotation) as GameObject;
			//tempWall.transform.localScale = new Vector3(wallX, wallY, 1);
		}


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
