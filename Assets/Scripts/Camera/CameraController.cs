using UnityEngine;
using System;
using System.Collections;

public class CameraController : MonoBehaviour {

	private float minCameraSize = 5;
	private float maxCameraSize = 50;

	private const float ViewPortEdge = 0.1f;
	private const float CameraMovementSpeed = 5.0f;
	private const float ViewEdgeDistance = 0.5f;

	public bool invertedScroll = false;
	public bool frozen = false;

	// Use this for initialization
	void Awake () {

		DebugRenderer.updateCamera(camera);
		//Initialize variables from file/options?

		//Set camera location and zoom based on map

		//Update max zoom based on map bounds.
	}

	void Start() {

		if(Options.gameMap != null && !Options.Testing) {
			Vector3 startPos = camera.transform.position;
			Vector3 penguinPos = Options.gameMap.map.cellIndexToWorld(Options.gameMap.map.PenguinSpawn);
			startPos.x = penguinPos.x;
			startPos.y = penguinPos.y;

			camera.transform.position = startPos;
			camera.orthographicSize = 30;
			DebugRenderer.updateLineWidth();
		}
	}

	void Update() {
		if(frozen || !Options.Testing) 
			return;

		updateCamera();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(Input.GetKeyUp(KeyCode.F3)) {
			frozen = !frozen;
		}
	
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
		
		if(Input.GetAxis("Mouse ScrollWheel") != 0) {

			float amountScroll = Input.GetAxis("Mouse ScrollWheel");
			float size = camera.orthographicSize + (invertedScroll? 1:-1) * amountScroll;
			camera.orthographicSize = Mathf.Clamp(size, minCameraSize, maxCameraSize);
			
			DebugRenderer.updateLineWidth();

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

		Vector3 mousePosition = camera.ScreenToViewportPoint(Input.mousePosition);
		Vector3 cameraPosition = camera.gameObject.transform.position;
		Vector2 moveAmount = Vector2.zero;

		if (mousePosition.x < ViewPortEdge && mousePosition.x >= camera.rect.x) {
			
			moveAmount.x = -getCameraSpeed(Math.Max(mousePosition.x, camera.rect.x)); //-((CameraMovementSpeed / camera.orthographicSize)* (0.1f - (mousePosition.x)));
		}
		else if (mousePosition.x > camera.rect.width - ViewPortEdge && mousePosition.x < camera.rect.width) {
			
			moveAmount.x = getCameraSpeed(camera.rect.width - Math.Min(mousePosition.x, camera.rect.width)); //((CameraMovementSpeed / camera.orthographicSize) * (0.1f - (1.0f - mousePosition.x)));
		}

		if (mousePosition.y < ViewPortEdge && mousePosition.y >= camera.rect.y) {
			
			moveAmount.y = -getCameraSpeed(Math.Max(mousePosition.y, camera.rect.y)); 
		}
		else if (mousePosition.y > camera.rect.height - ViewPortEdge && mousePosition.y < camera.rect.height) {//
			
			moveAmount.y = getCameraSpeed(camera.rect.height - Math.Min(mousePosition.y, camera.rect.height)); 
		}
		
		camera.gameObject.transform.position = clampCameraPosition(cameraPosition, moveAmount);
	}

	private float getCameraSpeed(float amount) {
		return ((CameraMovementSpeed) * (0.1f - amount));
	}

	public bool mouseInBounds() {

		Vector3 mousePosition = camera.ScreenToViewportPoint(Input.mousePosition);
		return (mousePosition.x >= camera.rect.x && mousePosition.x < camera.rect.width && mousePosition.y >= camera.rect.y && mousePosition.y < camera.rect.height);
	}

	private Vector3 clampCameraPosition(Vector3 cameraPosition, Vector2 moveAmount) {

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

		return cameraPosition;
	}
}
