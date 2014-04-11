using UnityEngine;
using System.Collections;

public class Infliction {
	
	public float damagePerInfliction {get; private set;}

	private int numberOfInflictions;
	private int currentInfliction;

	public Infliction(int numberOfInflictions, float damagePerInfliction) {
		currentInfliction = 0;

		this.numberOfInflictions = numberOfInflictions;
		this.damagePerInfliction = damagePerInfliction;
	}

	public bool inflict(Health health) {

		health.reduceHealth(damagePerInfliction);

		++currentInfliction;
		return currentInfliction != numberOfInflictions;
	}

}

