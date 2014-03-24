using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugRenderer {
	
	private static Texture2D lineTexture;
	private static Dictionary<float, Texture2D> circleTextures;
	
	public static float lineWidth;
	
	
	static DebugRenderer() {
		lineTexture = new Texture2D(1, 1);
		lineTexture.SetPixel(0,0, Color.black);
		lineTexture.Apply();
		
		circleTextures = new Dictionary<float, Texture2D>();
		
		lineWidth = DebugRenderer.worldToCameraLength(0.5f);
	}

	//Returns the circle texture of the given radius
	private static Texture2D getCircle(float radius) {
		
		if(!circleTextures.ContainsKey(radius))
			circleTextures[radius] = createCircle(radius);
		
		return circleTextures[radius];
	}

	//If circle texture of a given radius has not been created yet, creates said texture and returns
	private static Texture2D createCircle(float radius) {
		
		int ceilInt = Mathf.CeilToInt(radius);
		
		Texture2D circle = new Texture2D(ceilInt*2, ceilInt*2);
		
		for(int x = -ceilInt; x < ceilInt; ++x) {
			for(int y = -ceilInt; y < ceilInt; ++y) {
				
				int sqMag = x*x + y*y;
				float sqOuterRadius = radius*radius;
				float sqInnerRadius = (radius-lineWidth)*(radius-lineWidth);
				
				if(sqMag < sqOuterRadius && sqMag > sqInnerRadius) {
					circle.SetPixel(x+ceilInt,y+ceilInt, Color.black);
				}
				else {
					circle.SetPixel(x+ceilInt,y+ceilInt, Color.clear);
				}
			}
		}
		
		circle.Apply();
		
		return circle;
	}

	// Converts given world space length to camera space length
	public static float worldToCameraLength(float length) {
		return (Camera.main.WorldToScreenPoint(new Vector2(length, 0)) - Camera.main.WorldToScreenPoint(Vector2.zero)).x;
	}
	
	//Draw a circle at the given point. All in camera space.
	public static void drawCircle(Vector2 center, float radius) {
		Rect r = new Rect (center.x - radius, center.y - radius, radius * 2, radius * 2);
		Texture2D c = getCircle (radius);
		GUI.DrawTexture(r, c, ScaleMode.ScaleToFit);
	}
	
	//Draw a box
	public static void drawBox(float x, float y, float width, float height, float angle, Vector2 pivot){
		
		GUIUtility.RotateAroundPivot(angle, pivot);
		GUI.DrawTexture(new Rect(x, y, width, height), lineTexture, ScaleMode.StretchToFill);
		GUIUtility.RotateAroundPivot(-angle, pivot);
	}
}