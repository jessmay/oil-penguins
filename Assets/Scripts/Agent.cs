using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {

	protected double heading;
	protected Vector3 velocity;

	// Use this for initialization
	void Start () {
		heading = 0.0;
		velocity = new Vector3 (0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}

	protected void Move() {
		rigidbody2D.
		rigidbody2D.velocity = new Vector2 (velocity.x, velocity.y); 
		//transform.position += velocity;
		velocity = new Vector3(0, 0, 0);
	}
}
