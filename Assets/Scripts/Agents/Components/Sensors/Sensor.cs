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
	public bool displaySensor = false;

	public void toggleDisplay() {
		displaySensor = !displaySensor;
	}

	public abstract void drawDebugInformation();

	public void drawSensor() {
		if (displaySensor)
			drawDebugInformation();
	}

	protected string debugInformation;

	public string getDebugInformation() {
		return debugInformation;
	}
}
