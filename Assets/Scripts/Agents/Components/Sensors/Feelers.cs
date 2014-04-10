using UnityEngine;
using System.Collections;

public class Feelers : Sensor {
	
	public int numFeelers;
	public Vector2[] feelers;
	public float feelerLength;
	public int viewAngle;

	public Feelers (Agent me, float feelerLength, int numFeelers = 3, int viewAngle = 180) : base(me) {

		this.feelerLength = feelerLength;
		this.numFeelers = numFeelers;
		this.viewAngle = viewAngle;

		feelers = new Vector2[numFeelers];
	}

	public override void calculate() {
		
		// Save current object layer
		int oldLayer = me.gameObject.layer;

		int layerToIgnore = ~((1 << me.gameObject.layer) | (1 << LayerMask.NameToLayer("Ignore Raycast")));

		if(!Options.Testing) {
			//Change object layer to a layer it will be alone
			me.gameObject.layer = LayerMask.NameToLayer("RayCast");
			
			layerToIgnore = (1 << me.gameObject.layer) | (1 << LayerMask.NameToLayer("Ignore Raycast"));
			layerToIgnore = ~layerToIgnore;
		}


		float heading = me.getHeading();
		float radius = me.getRadius();

		float spaceBetween = viewAngle/(numFeelers+1);

		debugInformation = "";
		
		for (int currentFeeler = 0; currentFeeler < numFeelers; ++currentFeeler) {
			
			int angle = (int) (heading - viewAngle/2 + spaceBetween*(currentFeeler+1));
			
			angle = (angle + 360) % 360;
			
			Vector2 direction = new Vector2(-Mathf.Sin(Mathf.Deg2Rad*angle), Mathf.Cos(Mathf.Deg2Rad*angle));
			RaycastHit2D hit = Physics2D.Raycast((Vector2)(me.gameObject.renderer.bounds.center) + direction * radius, direction.normalized, feelerLength, layerToIgnore);
			feelers[currentFeeler] = hit.collider == null? direction*feelerLength: hit.fraction * direction*feelerLength;
			
			//Debug.DrawRay((Vector2)(gameObject.renderer.bounds.center) + direction * radius, feelers[currentFeeler], Color.black);
			
			//Add to debug information
			debugInformation += "Feeler["+currentFeeler+"]: "+ feelers[currentFeeler].magnitude +"\n";
		}
		
		// set the game object back to its original layer
		me.gameObject.layer = oldLayer;
	}

	public override void drawDebugInformation () {
		
		float spaceBetween = viewAngle/(feelers.Length+1);
		float heading = me.getHeading();
		
		float width = DebugRenderer.lineWidth;
		Vector2 center = me.getCenterCameraSpace();
		float radius = me.getRadiusCameraSpace();

		for (int currentFeeler = 0; currentFeeler < feelers.Length; ++currentFeeler) {
			
			int angle = (int) (heading - viewAngle/2 + spaceBetween*(currentFeeler+1));
			angle = (angle + 360) % 360;

			float feelerLengthCameraSpace = DebugRenderer.worldToCameraLength(feelers[currentFeeler].magnitude);

			Color color = Color.red;
			color.a = .5f;

			DebugRenderer.drawBox (center.x-width/2, center.y+radius, width, feelerLengthCameraSpace, -angle+180, center, color);
		}
	}
}
