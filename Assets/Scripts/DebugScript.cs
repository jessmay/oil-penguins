using UnityEngine;
using System.Collections;

public class DebugScript : MonoBehaviour {

	public GameObject Wall;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;
			Instantiate(Wall, pos, Wall.transform.rotation);
		}

	}
}
