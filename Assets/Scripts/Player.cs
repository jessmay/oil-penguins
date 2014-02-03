using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Agent {

	private const float turnStep = 5.0f;
	private const float moveStep = 10.0f;

	private int numFeelers = 3;
	private Vector2[] feelers;
	private int feelerLength;
	private bool dispFeelers = true;

	private int numSlices = 4;
	private bool dispSlices = true;

	private int viewAngle = 180;
	private int pieAngle = 360;

	private float lineWidth = .5f;

	public Texture feelerTex;

	private List<Agent> near;

	// Use this for initialization
	new void Start () {
		base.Start();
		feelerLength = (int)(radius*3);
	}
	
	// Update is called once per frame
	void Update () {

		//display feelers
		if (Input.GetKeyDown(KeyCode.F)) {
			dispFeelers = !dispFeelers;
		}

		//display pie slices
		if(Input.GetKeyDown(KeyCode.P)){
			dispSlices = !dispSlices;
		}

		//if turning CCW, increment heading
		if (Input.GetKey(KeyCode.LeftArrow)) {
			heading+=turnStep;
			if(heading >= 360) heading%=360;
			transform.rotation *= Quaternion.Euler (0,0, turnStep);
		}

		//If turning CW, decrement heading
		if (Input.GetKey(KeyCode.RightArrow)) {
			heading-=turnStep;
			if(heading < 0) heading = (heading+360)%360;
			transform.rotation *= Quaternion.Euler (0,0,-turnStep);
		}

		//If moving forward
		if (Input.GetKey(KeyCode.UpArrow)) {
			Vector2 temp = new Vector2(transform.up.x, transform.up.y);
			velocity = temp * (float)moveStep;
		}

		//If moving backward
		if (Input.GetKey(KeyCode.DownArrow)) {
			Vector2 temp = new Vector2(transform.up.x, transform.up.y);
			velocity = temp * (float)-moveStep;
		}

		feelers = getLengthOfFeelers(feelerLength, numFeelers, viewAngle);

		//displayFeelers(feelers);

		Move();
		near = background.grid.getNear (this, feelerLength);
		for (int i = 0; i < near.Count; ++i)
			print (near [i]);
	}

	void OnGUI(){
		
		if (dispFeelers) {
			
			Vector3 center = new Vector3(renderer.bounds.center.x, renderer.bounds.center.y);
			center.Scale(new Vector3(1, -1, 1));
			
			Vector2 pivot = (Vector2)Camera.main.WorldToScreenPoint(center);
			
			float spaceBetween = viewAngle/(feelers.Length+1);
			Vector2 radiusV = new Vector2(radius, 0);
			radiusV = (Camera.main.WorldToScreenPoint(radiusV) - Camera.main.WorldToScreenPoint(Vector2.zero));
			
			Vector2 width = new Vector2(lineWidth, 0);
			width = (Camera.main.WorldToScreenPoint(width) - Camera.main.WorldToScreenPoint(Vector2.zero));
			
			for (int currentFeeler = 0; currentFeeler < feelers.Length; ++currentFeeler) {

				int angle = (int) (heading - viewAngle/2 + spaceBetween*(currentFeeler+1));
				angle = (angle + 360) % 360;
				
				Vector2 feelerVec = new Vector2(feelers[currentFeeler].magnitude, 0);
				feelerVec = (Camera.main.WorldToScreenPoint(feelerVec) - Camera.main.WorldToScreenPoint(Vector2.zero));

				drawBox (pivot.x-width.x/2, pivot.y+radiusV.x, width.x, feelerVec.x, -angle+180, pivot);
			}
		}
		
		//TODO draw circle for nearest agents
		
		//TODO draw pie slices
		if(dispSlices) {
			
			Vector3 center = new Vector3(renderer.bounds.center.x, renderer.bounds.center.y);
			center.Scale(new Vector3(1, -1, 1));
			
			Vector2 pivot = (Vector2)Camera.main.WorldToScreenPoint(center);
			
			float spaceBetween = pieAngle/numSlices;
			Vector2 radiusV = new Vector2(radius, 0);
			radiusV = (Camera.main.WorldToScreenPoint(radiusV) - Camera.main.WorldToScreenPoint(Vector2.zero));
			
			Vector2 width = new Vector2(lineWidth, 0);
			width = (Camera.main.WorldToScreenPoint(width) - Camera.main.WorldToScreenPoint(Vector2.zero));

			Vector2 length = new Vector2(feelerLength, 0);
			length = (Camera.main.WorldToScreenPoint(length) - Camera.main.WorldToScreenPoint(Vector2.zero));

			for(int currSlice = 0; currSlice < numSlices; currSlice++){
				int angle = (int) ((heading-spaceBetween/2)+(currSlice*spaceBetween));
				angle = (angle+360)%360;

				drawBox (pivot.x-width.x/2, pivot.y+radiusV.x, width.x, length.x, -angle+180, pivot);
			}
			
		}
	}

	void drawBox(float x, float y, float width, float height, float angle, Vector2 pivot){
		GUIUtility.RotateAroundPivot(angle, pivot);
		
		GUI.DrawTexture(new Rect(x, y, width, height), feelerTex, ScaleMode.StretchToFill);//feelers[i].magnitude
		GUIUtility.RotateAroundPivot(-angle, pivot);
	}

}
