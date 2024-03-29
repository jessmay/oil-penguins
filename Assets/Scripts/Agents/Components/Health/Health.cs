using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Health {

	private List<Infliction> inflictions;
	public float currentHealth {get; private set;}
	public float maxHealth {get; private set;}

	private GameAgent agent;
	
	public Health(GameAgent agent, float maxHealth, float? startHealth = null) {
		
		this.agent = agent;
		this.maxHealth = maxHealth;
		
		if(startHealth.HasValue)
			currentHealth = startHealth.Value;
		else
			currentHealth = maxHealth;
		
		inflictions = new List<Infliction>();
	}
	

	public void Update() {

		//Iterate through inflictions in reverse
		for(int currInfliction = inflictions.Count-1; currInfliction >= 0; --currInfliction) {

			if(!inflictions[currInfliction].inflict(this))
				inflictions.RemoveAt(currInfliction);
		}

		if(currentHealth <= 0) {
			inflictions.Clear();
			agent.onDeath();
		}
	}

	public void drawHealthBar() {

		if(currentHealth <= 0 || currentHealth >= maxHealth)
			return;

		float size = agent.getRadiusCameraSpace()*2;

		Vector3 center = agent.getCenterCameraSpace();

		float height = DebugRenderer.worldToCameraLength(0.2f);

		DebugRenderer.drawBox(new Rect(center.x - size/2, center.y - height/2, Mathf.Max(1, size * (float)currentHealth / maxHealth), height), getHealthColor());

	}


	//TODO: smooth transition between colors.
	public Color getHealthColor() {

		if(currentHealth/maxHealth > 0.50f)
			return Color.green;
		else if (currentHealth/maxHealth > 0.30f)
			return Color.yellow;
		else 
			return Color.red;
	}

	public void addInfliction(Infliction infliction) {
		inflictions.Add(infliction);
	}

	public void reduceHealth(float amount) {
		currentHealth -= amount + 1E-5f;
	}

	public void restoreToFullHealth() {
		currentHealth = maxHealth;
	}
}

