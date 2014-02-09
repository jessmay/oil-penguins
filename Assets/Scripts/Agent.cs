/*
Jessica May
Joshua Linge
Agent.cs
 */

using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {

	public Background background;

	protected float heading;
	protected Vector2 velocity;

	protected Vector2 cellIndex;

	protected float radius;
	
	// Use this for initialization
	protected void Start() {
		heading = transform.rotation.z;
		velocity = Vector2.zero;
		radius = GetComponent<CircleCollider2D> ().radius;
		cellIndex = Vector2.zero;
	}

	protected void Move() {
		Vector2 prevCell = cellIndex;

		rigidbody2D.velocity = velocity; 
		velocity = Vector2.zero;

		cellIndex = background.grid.getCellIndex(renderer.bounds.center);
		if (cellIndex != prevCell) {
			background.grid.move(this, prevCell, cellIndex);
		}
	}

	protected Vector2[] getLengthOfFeelers(int range, int amount, int viewAngle = 180) {

		// Save current object layer
		int oldLayer = gameObject.layer;
		
		//Change object layer to a layer it will be alone
		gameObject.layer = LayerMask.NameToLayer("RayCast");
		
		int layerToIgnore = 1 << gameObject.layer;
		layerToIgnore = ~layerToIgnore;


		Vector2[] feelers =  new Vector2[amount];
		float spaceBetween = viewAngle/(amount+1);

		for (int currentFeeler = 0; currentFeeler < amount; ++currentFeeler) {

			int angle = (int) (heading - viewAngle/2 + spaceBetween*(currentFeeler+1));

			angle = (angle + 360) % 360;

			Vector2 direction = new Vector2(-Mathf.Sin(Mathf.Deg2Rad*angle), Mathf.Cos(Mathf.Deg2Rad*angle));
			RaycastHit2D hit = Physics2D.Raycast((Vector2)(renderer.bounds.center) + direction * radius, direction.normalized, range, layerToIgnore);
			feelers[currentFeeler] = hit.collider == null? direction*range: hit.fraction * direction*range;

			Debug.DrawRay((Vector2)(renderer.bounds.center) + direction * radius, feelers[currentFeeler], Color.black);
		}
		
		// set the game object back to its original layer
		gameObject.layer = oldLayer;

		return feelers;
	}

	public Vector2 getCellIndex () {
		return cellIndex;
	}

	public float getRadius () {
		return radius;
	}
}
