using UnityEngine;
using System.Collections;

public class Grid {

	private int width = 10;
	private int height = 10;
	private Bounds bounds;

	private float xSize;
	private float ySize;

	ArrayList[,] grid;

	public Grid(int w, int h, Bounds b){
		width = w;
		height = h;
		bounds = b;

		grid = new ArrayList[height,width];
		for (int i = 0; i < height; i++) {
			for(int j = 0; j < height; j++){
				grid[i,j] = new ArrayList();
			}
		}

		xSize = bounds.size.x/width;
		ySize = bounds.size.y/height;

	}

	public Vector2 getCellIndex(Vector2 coord){
		Vector2 cellIndex = new Vector2 ();
		cellIndex.x = Mathf.Clamp((coord.x - bounds.center.x) / xSize, 0, width-1);
		cellIndex.y = Mathf.Clamp((coord.y - bounds.center.y) / ySize, 0, height-1);

		return cellIndex;
	}


	public void add(GameObject a, Vector2 to){
		grid [(int)to.y,(int)to.x].Add (a);
	}

	public void add (GameObject a) {
		add (a, getCellIndex (a.renderer.bounds.center));
	}

	public void remove (GameObject a, Vector2 from) {
		grid [(int)from.y, (int)from.x].Remove (a);
	}

	public void remove (GameObject a) {
		remove (a, getCellIndex (a.renderer.bounds.center));
	}

	public void move (GameObject a, Vector2 from, Vector2 to) {
		remove (a, from);
		add (a, to);
	}

	//TODO fix range
	public ArrayList getNear (GameObject a, float radius) {
		ArrayList near = new ArrayList ();
		Vector2 lower = getCellIndex (a.renderer.bounds.center - new Vector3 (radius, radius, 0));
		Vector2 upper = getCellIndex (a.renderer.bounds.center + new Vector3 (radius, radius, 0));

		for (int i = (int)lower.y; i <= (int)upper.y; ++i) {
			for (int j = (int)lower.x; j <= (int)upper.x; ++j) {
				for (int k = 0; k < grid[i,j].Count; ++k) {

					GameObject b = grid[i,j][k] as GameObject;
					if (b.Equals(a)) continue;

					float dist = Vector3.Distance(a.renderer.bounds.center, b.renderer.bounds.center);

					if(dist <= radius ) {
						near.Add(b);
					}
				}
			}
		}

		return near;
	}

}
