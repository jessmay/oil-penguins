/*
Joshua Linge
DebugRenderer.cs

2014-03-17
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugRenderer {

	private static Texture2D lineTexture;
	private static Dictionary<Color, Dictionary<float, Texture2D>> circles;

	public static Camera currentCamera {get; private set;}

	private static float worldLineWidth = 0.5f;
	public static float lineWidth;

	//Initialize
	static DebugRenderer() {
		lineTexture = new Texture2D(1, 1);
		lineTexture.SetPixel(0,0, Color.white);
		lineTexture.Apply();

		circles = new Dictionary<Color, Dictionary<float, Texture2D>>();
	}

	public static void updateCamera(Camera camera) {

		currentCamera = camera;
		updateLineWidth();
	}

	public static void updateLineWidth() {
		lineWidth = DebugRenderer.worldToCameraLength(worldLineWidth);
	}

	//Retrives a circle of the given radius and color. 
	//If the circle does not exist, create it.
	private static Texture2D getCircle(float radius, Color color) {

		if(!circles.ContainsKey(color)) {
			circles[color] = new Dictionary<float, Texture2D>(); 
		}

		if(!circles[color].ContainsKey(radius))
			circles[color][radius] = createCircle(radius, color);
		
		return circles[color][radius];

	}

	//Create a texture of a circle with the given radius and color.
	private static Texture2D createCircle(float radius, Color color) {

		int ceilInt = Mathf.CeilToInt(radius);

		Texture2D circle = new Texture2D(ceilInt*2, ceilInt*2);

		for(int x = -ceilInt; x < ceilInt; ++x) {
			for(int y = -ceilInt; y < ceilInt; ++y) {

				int sqMag = x*x + y*y;
				float sqOuterRadius = radius*radius;
				float sqInnerRadius = (radius-lineWidth)*(radius-lineWidth);

				if(sqMag < sqOuterRadius && sqMag > sqInnerRadius) {
					circle.SetPixel(x+ceilInt,y+ceilInt, color);
				}
				else {
					circle.SetPixel(x+ceilInt,y+ceilInt, Color.clear);
				}
			}
		}

		circle.Apply();

		return circle;
	}

	//Convert the given length from world space to camera space
	public static float worldToCameraLength(float length) {

		Camera camera = currentCamera;

		//Camera camera = c.HasValue? c.Value: DebugRenderer.currentCamera;
		return (camera.WorldToScreenPoint(new Vector2(length, 0)) - camera.WorldToScreenPoint(Vector2.zero)).x;
	}

	//Draw a circle at the given point. All in camera space.
	public static void drawCircle(Vector2 center, float radius, Color? c = null) {

		Color color = c.HasValue ? c.Value : Color.black;

		GUI.DrawTexture(new Rect(center.x-radius, center.y-radius, radius*2, radius*2), getCircle(radius, color), ScaleMode.ScaleToFit);
	}
	
	//Draw a box
	public static void drawBox(float x, float y, float width, float height, float angle, Vector2 pivot, Color? c = null){

		Color color = c.HasValue ? c.Value : Color.black;
		
		GUIUtility.RotateAroundPivot(angle, pivot);
		GUI.DrawTexture(new Rect(x, y, width, height), colorizeTexture(lineTexture, color), ScaleMode.StretchToFill);
		GUIUtility.RotateAroundPivot(-angle, pivot);
	}

	
	public static Texture2D colorizeTexture(Texture2D originalTexture, Color color) {
		
		int width = originalTexture.width;
		int height = originalTexture.height;
		Texture2D colorizedTexture = new Texture2D(width, height);
		
		for (int x = 0; x < width; ++x) {
			for (int y = 0; y < height; ++y) {
				colorizedTexture.SetPixel(x, y, originalTexture.GetPixel(x,y) * color);
			}
		}
		
		colorizedTexture.Apply();
		colorizedTexture.filterMode = originalTexture.filterMode;
		
		return colorizedTexture;
	}
}
