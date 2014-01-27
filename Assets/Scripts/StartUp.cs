using UnityEngine;
using System.Collections;

public class StartUp : MonoBehaviour {

	public GameObject Wall;
	private GameObject[][] board;
	private int width = 10;
	private int height = 10;

	// Use this for initialization
	void Start () {
		//board = new GameObject[height][width];

		//float x = renderer.bounds.size.x;
		//float y = renderer.bounds.size.y;

		Instantiate (Wall, Vector3.zero, Wall.transform.rotation);


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
