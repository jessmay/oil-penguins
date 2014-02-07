using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid {

	private int width = 10;
	private int height = 10;
	private Bounds bounds;

	private float xSize;
	private float ySize;

	private float maxAgentRadius;

	List<Agent>[,] grid;

	public Grid(int w, int h, Bounds b){

		width = w;
		height = h;
		bounds = b;

		maxAgentRadius = 0;

		grid = new List<Agent>[height,width];
		for (int i = 0; i < height; i++) {
			for(int j = 0; j < height; j++){
				grid[i,j] = new List<Agent>();
			}
		}

		xSize = bounds.size.x/width;
		ySize = bounds.size.y/height;

	}

	//Check to see if the cell index is within the bounds of the grid
	public bool inBounds (Vector2 coord) {
		return coord.x >= 0 && coord.x < width && coord.y >= 0 && coord.y < height;
	}

	//Check to see if the cell index is within the bounds of the grid
	public bool inBounds (int x, int y) {
		return x >= 0 && x < width && y >= 0 && y < height;
	}
	
	//Given a world coordinate, translate it into an index into the grid.
	public Vector2 getCellIndex(Vector2 coord){
		Vector2 cellIndex = new Vector2 ();

		cellIndex.x = (int)((coord.x - bounds.center.x) / xSize + width/2);
		cellIndex.y = (int)((coord.y - bounds.center.y) / ySize + height/2);

		return cellIndex;
	}

	//Add an agent to the grid.
	//Returns true if agent was added.
	//Returns false if location is out of bounds or agent is already within the grid.
	public bool add(Agent a, Vector2 to){

		if (!inBounds(to) || grid [(int)to.y,(int)to.x].Contains(a))
			return false;

		maxAgentRadius = Mathf.Max(maxAgentRadius, a.getRadius());
		grid [(int)to.y,(int)to.x].Add (a);
		return true;
	}

	//Add an agent to the grid.
	//Returns true if agent was added.
	//Returns false if location is out of bounds or agent is already within the grid.
	public bool add (Agent a) {
		return add (a, getCellIndex (a.renderer.bounds.center));
	}

	public bool remove (Agent a, Vector2 from) {

		if (!inBounds(from) || !grid [(int)from.y, (int)from.x].Contains(a))
			return false;

		grid [(int)from.y, (int)from.x].Remove (a);
		return true;

	}

	public bool remove (Agent a) {
		return remove (a, getCellIndex (a.renderer.bounds.center));
	}

	public void move (Agent a, Vector2 from, Vector2 to) {
		remove (a, from);
		add (a, to);
	}

	public List<Agent> getNear (Agent a, float radius) {
		List<Agent> near = new List<Agent> ();

		float myRadius = a.getRadius();
		radius += myRadius;

		Vector2 lower = getCellIndex (a.renderer.bounds.center - new Vector3 (radius+ maxAgentRadius, radius+ maxAgentRadius, 0));
		Vector2 upper = getCellIndex (a.renderer.bounds.center + new Vector3 (radius+ maxAgentRadius, radius+ maxAgentRadius, 0));

		for (int i = (int)lower.y; i <= (int)upper.y; ++i) {
			for (int j = (int)lower.x; j <= (int)upper.x; ++j) {

				if (!inBounds(i,j))
					continue;

				for (int k = 0; k < grid[i,j].Count; ++k) {

					Agent b = grid[i,j][k];
					if (b.Equals(a)) continue;

					float theirRadius = b.getRadius();
					float dist = Vector3.Distance(a.renderer.bounds.center, b.renderer.bounds.center);

					if(dist <= radius + theirRadius) {
						near.Add(b);
					}
				}
			}
		}

		return near;
	}

}
