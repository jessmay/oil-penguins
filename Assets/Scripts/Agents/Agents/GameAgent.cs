using UnityEngine;
using System.Collections;

public abstract class GameAgent : Agent {

	protected Health health;

	protected abstract float getMaxHealth();
	public abstract void onDeath();

	protected override void initializeAgent () {
		health = new Health(this, getMaxHealth());
	}

	protected override void updateAgent () {
		health.Update();
	}

	protected override void drawStatus() {
		health.drawHealthBar();
	}

	public void addInfliction(Infliction infliction) {
		health.addInfliction(infliction);
	}
}

