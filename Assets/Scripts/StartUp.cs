using UnityEngine;
using System.Collections;

public class StartUp : MonoBehaviour {

	public GameObject Wall;
	private int width = 10;
	private int height = 10;

	// Use this for initialization
	void Start () {
		
		float xSize = renderer.bounds.size.x/width;
		float ySize = renderer.bounds.size.y/height;

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
	
	}
}
