using UnityEngine;
using System.Collections;

public class IcicleClub : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collider) {

		// Ignore if collision is with icicle penguin.
		if(collider.gameObject.GetComponent<IciclePenguins>() != null)
			return;

		GameObject target = collider.gameObject;
		
		//Deal damage
		GameAgent gameAgent = target.GetComponent<GameAgent>();
		if(gameAgent != null) {
			gameAgent.addInfliction(new Infliction(200, 0.10f));
		}
		
	}

}
