using UnityEngine;
using System.Collections;

public class ICEMachine : MonoBehaviour {

	public bool held {get; private set;}
	public GameObject holder {get; private set;}

	// Use this for initialization
	void Start () {
		held = false;
	}

	// Update is called once per frame
	void FixedUpdate () {

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
	}

	public void drop() {

		transform.position = holder.transform.position;

		held = false;
		holder = null;
	}

	void OnDestroy() {

		if(held) {
			holder.GetComponent<HumanAgent>().drop();
		}
	}
}

