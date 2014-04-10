using UnityEngine;
using System.Collections;

public class ICEMachine : MonoBehaviour {

	private bool held;
	private GameObject holder;

	// Use this for initialization
	void Start () {
		held = false;
	}

	// Update is called once per frame
	void Update () {

		if(held) {
			transform.position = holder.transform.position + holder.transform.up*((transform.localScale.x*((BoxCollider2D)collider2D).size.x)/2 + holder.gameObject.GetComponent<Agent>().getRadius());
			transform.rotation = holder.transform.rotation;
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {

		HumanAgent humanAgent = collider.gameObject.GetComponent<HumanAgent>();

		if(humanAgent == null || held)
			return;

		held = true;
		humanAgent.pickUp();
		holder = collider.gameObject;

		Debug.Log("Hit");
	}

	public void drop() {

		transform.position = holder.transform.position;

		held = false;
		holder = null;
	}
}

