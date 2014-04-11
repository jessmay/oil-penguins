using UnityEngine;
using System.Collections;

public class Tranquilizer : MonoBehaviour {

	private int endFrame;
	private static int lifeSpan = 1000;

	private bool hit;
	private GameObject target;
	private float hitDistance;
	private float hitAngle;

	private Quaternion hitRotation;
	private Vector3 hitVector;

	// Use this for initialization
	void Start () {

		hit = false;
		endFrame = Time.frameCount + lifeSpan;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		//Lived its life.
		if(Time.frameCount > endFrame)
			Destroy(gameObject);

		//Has hit a target, move with target.
		if(hit) {

			//Target was destroyed
			if(target == null){
				Destroy(gameObject);
			}
			//Target exists, move with target.
			else {
				transform.position = target.transform.position + (target.transform.rotation * hitVector) + transform.up*0.05f;
				transform.rotation = hitRotation * target.transform.rotation;
			}
		}
		//Move forward
		else {
			transform.position = transform.position + transform.up* 0.5f ;
		}

	}


	void OnTriggerEnter2D(Collider2D collider) {

		if(hit || collider.gameObject.GetComponent<Tranquilizer>() != null)
			return;
		
		hit = true;
		target = collider.gameObject;
		
		hitVector = Quaternion.Inverse(target.transform.rotation) * (transform.position - target.transform.position);

		//Stick out from agents
		if(collider.gameObject.GetComponent<Agent>() != null)
			hitRotation = Quaternion.Inverse(target.transform.rotation) * Quaternion.LookRotation(Vector3.forward, target.transform.position - transform.position);//Quaternion.Inverse(target.transform.rotation) * transform.rotation;

		//Keep rotation for non agents. (walls)
		else 
			hitRotation = Quaternion.Inverse(target.transform.rotation) * transform.rotation;

		//Deal damage
		//Affect humans too?

//		GameAgent gameAgent = target.GetComponent<GameAgent>();
//		if(gameAgent != null) {
//			gameAgent.addInfliction(new Infliction(100, 0.30f));
//		}

		if(collider.gameObject.GetComponent<HumanAgent>() != null) {
			collider.gameObject.GetComponent<HumanAgent>().health.addInfliction(new Infliction(100, 0.30f));
		}

	}


}
