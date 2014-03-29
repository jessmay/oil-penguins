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

	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		if(frozen)
			return;

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

		if (mousePosition.x < ViewPortEdge && mousePosition.x >= camera.rect.x) {
			
			cameraPosition.x += -getCameraSpeed(Math.Max(mousePosition.x, camera.rect.x)); //-((CameraMovementSpeed / camera.orthographicSize)* (0.1f - (mousePosition.x)));
		}
		else if (mousePosition.x > camera.rect.width - ViewPortEdge && mousePosition.x < camera.rect.width) {
			
			cameraPosition.x += getCameraSpeed(camera.rect.width - Math.Min(mousePosition.x, camera.rect.width)); //((CameraMovementSpeed / camera.orthographicSize) * (0.1f - (1.0f - mousePosition.x)));
		}

		if (mousePosition.y < ViewPortEdge && mousePosition.y >= camera.rect.y) {
			
			cameraPosition.y += -getCameraSpeed(Math.Max(mousePosition.y, camera.rect.y)); 
		}
		else if (mousePosition.y > camera.rect.height - ViewPortEdge && mousePosition.y < camera.rect.height) {//
			
			cameraPosition.y += getCameraSpeed(camera.rect.height - Math.Min(mousePosition.y, camera.rect.height)); 
		}
		
		camera.gameObject.transform.position = clampCameraPosition(cameraPosition);
	}

	private float getCameraSpeed(float amount) {
		return ((CameraMovementSpeed) * (0.1f - amount));
	}

	public bool mouseInBounds() {

		Vector3 mousePosition = camera.ScreenToViewportPoint(Input.mousePosition);
		return (mousePosition.x >= camera.rect.x && mousePosition.x < camera.rect.width && mousePosition.y >= camera.rect.y && mousePosition.y < camera.rect.height);
	}

	private Vector3 clampCameraPosition(Vector3 cameraPosition) {

		Bounds mapBounds = Options.gameMap.map.getBounds();

		float zoom = camera.orthographicSize;

		cameraPosition.x = Mathf.Clamp(cameraPosition.x, -mapBounds.extents.x + zoom * ViewEdgeDistance, mapBounds.extents.x - zoom * ViewEdgeDistance);
		cameraPosition.y = Mathf.Clamp(cameraPosition.y, -mapBounds.extents.y + zoom * ViewEdgeDistance, mapBounds.extents.y - zoom * ViewEdgeDistance);

		//Debug.Log(cameraPosition);

		return cameraPosition;
	}
}
