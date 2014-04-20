using UnityEngine;
using System.Collections;

public class IcicleClub : MonoBehaviour {

	public IciclePenguins penguin;

	void OnTriggerEnter2D(Collider2D collider) {

		// Ignore if collision is with icicle penguin.
		if(collider.gameObject.GetComponent<IciclePenguins>() != null || !((IciclePenguinAttackState)penguin.IPfsm.getState(typeof(IciclePenguinAttackState))).rotating)
			return;

		GameObject target = collider.gameObject;
		
		//Deal damage
		GameAgent gameAgent = target.GetComponent<GameAgent>();
		if(gameAgent != null) {
			gameAgent.addInfliction(new Infliction(10, 2.5f));
		}
		
	}

}
