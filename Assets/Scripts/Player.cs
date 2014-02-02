using UnityEngine;
using System.Collections;

public class Player : Agent {

	private const float turnStep = 5.0f;
	private const float moveStep = 10.0f;//0.15f;

	private int numFeelers = 3;
	private Vector2[] feelers;
	private int feelerLength;
	private bool dispFeelers = true;

	private int viewAngle = 180;

	private float feelerWidth = .5f;

	public Texture feelerTex;

	// Use this for initialization
	void Start () {
		base.Start();
		feelerLength = (int)(GetComponent<CircleCollider2D>().radius*3);
	}
	
	// Update is called once per frame
	void Update () {

		//display feelers
		if (Input.GetKeyDown(KeyCode.F)) {
			dispFeelers = !dispFeelers;
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
	}

	void OnGUI(){
		
		if (dispFeelers) {
			
			Vector3 center = new Vector3(renderer.bounds.center.x, renderer.bounds.center.y);
			center.Scale(new Vector3(1, -1, 1));
			
			Vector2 pivot = (Vector2)Camera.main.WorldToScreenPoint(center);
			
			float spaceBetween = viewAngle/(feelers.Length+1);
			Vector2 radius = new Vector2(GetComponent<CircleCollider2D>().radius, 0);
			radius = (Camera.main.WorldToScreenPoint(radius) - Camera.main.WorldToScreenPoint(Vector2.zero));
			
			Vector2 width = new Vector2(feelerWidth, 0);
			width = (Camera.main.WorldToScreenPoint(width) - Camera.main.WorldToScreenPoint(Vector2.zero));
			
			for (int currentFeeler = 0; currentFeeler < feelers.Length; ++currentFeeler) {
				//int currentFeeler = 1; {	
				int angle = (int) (heading - viewAngle/2 + spaceBetween*(currentFeeler+1));
				angle = (angle + 360) % 360;
				
				Vector2 feelerVec = new Vector2(feelers[currentFeeler].magnitude, 0);
				feelerVec = (Camera.main.WorldToScreenPoint(feelerVec) - Camera.main.WorldToScreenPoint(Vector2.zero));
				
				GUIUtility.RotateAroundPivot(-angle+180, pivot);
				//GUI.Box(new Rect(pivot.x-width.x/2, pivot.y+radius.x, width.x, feelerVec.x), "");//feelers[i].magnitude
				GUI.DrawTexture(new Rect(pivot.x-width.x/2, pivot.y+radius.x, width.x, feelerVec.x), feelerTex, ScaleMode.StretchToFill);//feelers[i].magnitude
				GUIUtility.RotateAroundPivot(+angle-180, pivot);
				
			}
		}
	}

}
