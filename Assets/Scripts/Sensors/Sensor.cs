using UnityEngine;
using System.Collections;

public abstract class Sensor {

	//Agent that holds this sensor
	protected Agent me;
	
	//Calculate information for this sensor
	public abstract void calculate();

	public Sensor (Agent me) {
		this.me = me;
	}


	//Debug
	public abstract void drawDebugInformation();

	protected string debugInformation;

	public string getDebugInformation() {
		return debugInformation;
	}
}
