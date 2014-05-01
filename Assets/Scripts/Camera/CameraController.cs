using UnityEngine;
using System;
using System.Collections;

public class CameraController : MonoBehaviour {

	public static float minCameraSize = 3;
	public static float maxCameraSize = 125;

	private const float ViewPortEdge = 0.1f;
	private const float CameraMovementSpeed = 0.5f;
	private const float ViewEdgeDistance = 0.5f;

	
	private Rect screenDisplay;
	private const float moveEdgePixels = 100;

	public bool invertedScroll = false;
	public bool frozen = false;

	private float scrollAmount;
	private int prevScrollFrame;


	// Use this for initialization
	void Awake () {


		DebugRenderer.updateCamera(camera);

	}

	void Start() {

		if(Options.play || Options.mapEditing) {
			camera.rect = new Rect(camera.rect.x, camera.rect.y + PlayGameGUI.GUISize/(float)Screen.height, camera.rect.width, (Screen.height - PlayGameGUI.GUISize)/(float)Screen.height);
		}


		screenDisplay = new Rect(Screen.width * camera.rect.x, Screen.height * camera.rect.y, Screen.width * camera.rect.width, Screen.height * camera.rect.height);;

		prevScrollFrame = Time.frameCount;

		//Set camera location and zoom based on map
		if(Options.gameMap != null && !Options.Testing) {
			Vector3 startPos = camera.transform.position;
			Vector3 penguinPos = Options.gameMap.map.cellIndexToWorld(Options.gameMap.map.PenguinSpawn);
			startPos.x = penguinPos.x;
			startPos.y = penguinPos.y;

			camera.transform.position = startPos;
			camera.orthographicSize = 30;
			DebugRenderer.updateLineWidth();
		}
		
		//Initialize variables from file/options?

		//Update max zoom based on map bounds.
		if(Options.gameMap != null) {
			Bounds mapBounds = Options.gameMap.map.getBounds();
			maxCameraSize = Mathf.Max(mapBounds.extents.x * Screen.height/ Screen.width, mapBounds.extents.y) + 10;
		}
	}

	float tempScrollAmount = 0;
	void Update() {
		
		if(Input.GetKeyUp(KeyCode.F3)) {
			frozen = !frozen;
		}

		//TODO: Make platform independent.

		tempScrollAmount += Input.GetAxis("Mouse ScrollWheel");
		if(prevScrollFrame + 10 < Time.frameCount) {

			scrollAmount = tempScrollAmount != 0? 10 * Mathf.Sign(tempScrollAmount) : 0;

			prevScrollFrame = Time.frameCount;
			tempScrollAmount = 0;
		}

		if(frozen || !Options.Testing) 
			return;

		updateCamera();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(frozen || Options.Testing)
			return;

		updateCamera();
	}

	private void updateCamera() {
		
		zoom();
		
		pan();
	}

	//Increase or decrease orthographic size (zoom) based on mouse scroll wheel.
	private void zoom () {
		
		if(scrollAmount != 0) {

			float size = camera.orthographicSize + (invertedScroll? 1:-1) * Mathf.Sign(scrollAmount)/2;
			camera.orthographicSize = Mathf.Clamp(size, minCameraSize, maxCameraSize);
			
			DebugRenderer.updateLineWidth();
			scrollAmount = scrollAmount - Mathf.Sign(scrollAmount);

//			if (size > minCameraSize && size < maxCameraSize) {
//
//				Vector3 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
//				Vector3 cameraPosition = camera.gameObject.transform.position;
//				mousePosition.z = cameraPosition.z;
//
//				Vector3 dif = (mousePosition - cameraPosition);
//
//				camera.gameObject.transform.position = clampCameraPosition(cameraPosition + amountScroll * dif.normalized * (!invertedScroll? 1:-1));
//			}
		}

	}


	//Move the camera when the mouse is within the view port percentage.	
	private void pan () {

		if (!mouseInBounds())
			return;

		Vector3 mousePosition = Input.mousePosition;
		Vector3 cameraPosition = camera.gameObject.transform.position;
		Vector2 moveAmount = Vector2.zero;

		//Left
		if (mousePosition.x < screenDisplay.x + moveEdgePixels && mousePosition.x >= screenDisplay.x) {

			float percent = (moveEdgePixels - (mousePosition.x - screenDisplay.xMin))/ moveEdgePixels;
			moveAmount.x = -CameraMovementSpeed * percent;
		}
		//Right
		else if (mousePosition.x < screenDisplay.x + screenDisplay.width && mousePosition.x >= screenDisplay.x + screenDisplay.width - moveEdgePixels) {

			float percent = (moveEdgePixels - (screenDisplay.xMax - mousePosition.x))/ moveEdgePixels;
			moveAmount.x = CameraMovementSpeed * percent;
		}

		//Down
		if (mousePosition.y < screenDisplay.y + moveEdgePixels && mousePosition.y >= screenDisplay.y) {

			float percent = (moveEdgePixels - (mousePosition.y - screenDisplay.yMin))/ moveEdgePixels;
			moveAmount.y = -CameraMovementSpeed * percent;
		}
		//Up
		else if (mousePosition.y < screenDisplay.y + screenDisplay.width && mousePosition.y >= screenDisplay.y + screenDisplay.height - moveEdgePixels) {

			float percent = (moveEdgePixels - (screenDisplay.yMax - mousePosition.y))/ moveEdgePixels;
			moveAmount.y = CameraMovementSpeed * percent;
		}
		
		moveCameraClamped(cameraPosition, moveAmount);
	}

	private float getCameraSpeed(float amount) {
		return ((CameraMovementSpeed) * (0.1f - amount));
	}

	public bool mouseInBounds() {

		Vector3 mousePosition = Input.mousePosition;
		return (mousePosition.x >= screenDisplay.xMin && mousePosition.x < screenDisplay.xMax && mousePosition.y >= screenDisplay.yMin && mousePosition.y < screenDisplay.yMax);
	}

	public void moveCameraClamped(Vector3 cameraPosition, Vector2 moveAmount) {

		Bounds mapBounds = Options.gameMap.map.getBounds();

		//float zoom = camera.orthographicSize;

		//Vector2 min = new Vector2(-mapBounds.extents.x + zoom * ViewEdgeDistance, -mapBounds.extents.y + zoom * ViewEdgeDistance);
		//Vector2 max = new Vector2(mapBounds.extents.x - zoom * ViewEdgeDistance, mapBounds.extents.y - zoom * ViewEdgeDistance);

		//camera.WorldToScreenPoint(mapBounds.min);

		//TODO: Limit camera to not allow more than 100(?) pixels outside of the map.

		Vector2 min = new Vector2(-mapBounds.extents.x, -mapBounds.extents.y);
		Vector2 max = new Vector2(mapBounds.extents.x, mapBounds.extents.y);

		cameraPosition.x = Mathf.Clamp(cameraPosition.x + moveAmount.x, min.x, max.x);
		cameraPosition.y = Mathf.Clamp(cameraPosition.y + moveAmount.y, min.y, max.y);

		//Debug.Log(min +" < " +cameraPosition +" < " +max);

		camera.gameObject.transform.position =  cameraPosition;
	}
}
