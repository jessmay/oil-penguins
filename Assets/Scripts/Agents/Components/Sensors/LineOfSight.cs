using UnityEngine;
using System.Collections;

public class LineOfSight : Sensor {

	private RaycastHit2D raycast;
	public bool inSight;

	private Vector2 direction;

	public LineOfSight(TestableAgent me) : base(me) {
		displaySensor = true;
	}

	public override void calculate() {

		TestableAgent me = (TestableAgent)this.me;

		// Save current object layer
		int oldLayer = me.gameObject.layer;
		
		int layerToIgnore = ~(1 << me.gameObject.layer);
		
		if(!Options.Testing) {
			//Change object layer to a layer it will be alone
			me.gameObject.layer = LayerMask.NameToLayer("RayCast");
			
			layerToIgnore = 1 << me.gameObject.layer;
			layerToIgnore = ~layerToIgnore;
		}
		
		direction = me.getTarget() - (Vector2)me.transform.position;
		
		raycast = Physics2D.Raycast((Vector2)(me.renderer.bounds.center) + direction.normalized * me.getRadius(), direction.normalized, direction.magnitude,layerToIgnore);
		
		// set the game object back to its original layer
		me.gameObject.layer = oldLayer;
		
		inSight = raycast.collider == null;
	}

	public override void drawDebugInformation () {

		float width = DebugRenderer.lineWidth;
		Vector2 center = me.getCenterCameraSpace();
		float radius = me.getRadiusCameraSpace();

		Color color = Color.magenta;
		color.a = .5f;

		float hitLength = DebugRenderer.worldToCameraLength(direction.magnitude * (inSight?1:raycast.fraction));

		DebugRenderer.drawBox (center.x-width/2, center.y+radius, width, hitLength, -Vector2.Angle(-Vector2.up, direction), center, color);
	}

}

