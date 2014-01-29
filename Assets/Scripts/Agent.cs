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
		velocity = Vector2.zero;
	}

	protected float[] getLengthOfFeelers(int range, int amount, int viewAngle = 180) {

		float[] feelers =  new float[amount];
		float spaceBetween = viewAngle/(amount+1);

		for (int currentFeeler = 1; currentFeeler <= amount; ++currentFeeler) {

			int angle = (int) (heading - viewAngle + spaceBetween*currentFeeler);

			angle = (angle + 360) % 360;

			Vector3 vec = new Vector3(Mathf.Cos(Mathf.Deg2Rad*angle), Mathf.Sin(Mathf.Deg2Rad*angle), 0) * range;
			print (currentFeeler +" " +vec);
			Debug.DrawRay(renderer.bounds.center, vec, Color.black);
		}
		return feelers;
	}

}
