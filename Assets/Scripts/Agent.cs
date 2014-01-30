using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {

	protected float heading;
	protected Vector2 velocity;
	
	// Use this for initialization
	protected void Start() {
		heading = transform.rotation.z;
		velocity = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update() {

	}

	protected void Move() {
		rigidbody2D.velocity = velocity; 
		velocity = Vector2.zero;
	}

	protected float[] getLengthOfFeelers(int range, int amount, int viewAngle = 180) {

		// Save current object layer
		int oldLayer = gameObject.layer;
		
		//Change object layer to a layer it will be alone
		gameObject.layer = LayerMask.NameToLayer("RayCast");
		
		int layerToIgnore = 1 << gameObject.layer;
		layerToIgnore = ~layerToIgnore;


		float[] feelers =  new float[amount];
		float spaceBetween = viewAngle/(amount+1);
		float radius = GetComponent<CircleCollider2D>().radius;

		for (int currentFeeler = 0; currentFeeler < amount; ++currentFeeler) {

			int angle = (int) (heading - viewAngle/2 + spaceBetween*(currentFeeler+1));

			angle = (angle + 360) % 360;

			Vector3 direction = new Vector3(-Mathf.Sin(Mathf.Deg2Rad*angle), Mathf.Cos(Mathf.Deg2Rad*angle), 0);
			RaycastHit2D hit = Physics2D.Raycast(renderer.bounds.center + direction * radius, direction.normalized, range, layerToIgnore);
			feelers[currentFeeler] = hit.collider == null? range: hit.fraction * range;

			Debug.DrawRay(renderer.bounds.center + direction * radius, direction * feelers[currentFeeler], Color.black);
		}
		
		// set the game object back to its original layer
		gameObject.layer = oldLayer;

		return feelers;
	}

	protected void displayFeelers(float[] feelers, int viewAngle = 180) {

		float spaceBetween = viewAngle/(feelers.Length+1);
		float radius = GetComponent<CircleCollider2D>().radius;

		for (int currentFeeler = 0; currentFeeler < feelers.Length; ++currentFeeler) {
			
			int angle = (int) (heading - viewAngle/2 + spaceBetween*(currentFeeler+1));
			angle = (angle + 360) % 360;

			Vector3 direction = new Vector3(-Mathf.Sin(Mathf.Deg2Rad*angle), Mathf.Cos(Mathf.Deg2Rad*angle), 0);
			Debug.DrawRay(renderer.bounds.center + direction * radius, direction * feelers[currentFeeler], Color.black);
			LineRenderer line = GameObject.Find("LineRenderer").GetComponent<LineRenderer>();
			line.SetPosition(0,renderer.bounds.center + direction * radius);
			line.SetPosition(1,renderer.bounds.center + direction * radius + direction * feelers[currentFeeler]);
		}

	}

}
