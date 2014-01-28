using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {

	protected double heading;
	protected Vector2 velocity;

	// Use this for initialization
	void Start() {
		heading = 0.0;
		velocity = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update() {
		//Move();
	}

	protected void Move() {
		rigidbody2D.velocity = velocity; 
		//transform.position += velocity;
		velocity = Vector2.zero;
	}
}
