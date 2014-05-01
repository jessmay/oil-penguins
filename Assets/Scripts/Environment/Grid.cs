/*
Jessica May
Joshua Linge
Grid.cs

Updated by Joshua Linge on 2014-03-17
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Grid : IDisposable {

	private int width = 10;
	private int height = 10;
	private Bounds bounds;
	private Vector2 center;

	private float xSize;
	private float ySize;

	private float maxAgentRadius;

	public Map map;

	List<Agent>[,] grid;

	public Grid (Map map) {
		this.map = map;

		initialize(map.getMapWidth(), map.getMapHeight(), map.getBounds());
	}

	public Grid(int w, int h, Bounds b){
		initialize(w, h, b);
	}

	private void initialize(int w, int h, Bounds b) {
		
		width = w;
		height = h;
		bounds = b;
		center = b.center;
		
		maxAgentRadius = 0;
		
		grid = new List<Agent>[height,width];
		for (int i = 0; i < height; i++) {
			for(int j = 0; j < width; j++){
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

		cellIndex.x = Mathf.FloorToInt((float)((coord.x - center.x + (width%2 == 1? xSize/2.0:0)) / xSize + width/2));
		cellIndex.y = Mathf.FloorToInt((float)((coord.y - center.y + (height%2 == 1? xSize/2.0:0)) / ySize + height/2));

		return cellIndex;
	}
	
	//Given a cell index, translate into a world coordinate centered at that cell.
	public Vector3 cellIndexToWorld (Vector2 coord) {

		Vector3 worldPoint = new Vector3(0,0,0);
		
		worldPoint.x = center.x+xSize/2+(coord.x-width/2)*xSize - (width%2 == 1? xSize/2:0);
		worldPoint.y = center.y+ySize/2+(coord.y-height/2)*ySize - (height%2 == 1? ySize/2:0);
		
		return worldPoint;
	}


	//Add an agent to the grid.
	//Returns true if agent was added.
	//Returns false if location is out of bounds or agent is already within the grid.
	public bool add(Agent a, Vector2 to){

		if (!inBounds(to) || grid [(int)to.y,(int)to.x].Contains(a))
			return false;

		maxAgentRadius = Mathf.Max(maxAgentRadius, a.getRadius());
		grid [(int)to.y,(int)to.x].Add (a);

		if(grid [(int)to.y, (int)to.x].Count == 1)
			map.canMove[(int)to.x,(int)to.y] = false;

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

		if(grid [(int)from.y, (int)from.x].Count == 0)
			map.canMove[(int)from.x,(int)from.y] = true;

		return true;
	}

	private void removeAllAgents() {
		for (int i = 0; i < height; i++) {
			for(int j = 0; j < width; j++){

				foreach (Agent agent in grid[i,j])
					GameObject.Destroy(agent.gameObject);
				grid[i,j].Clear();
			}
		}
	}

	public bool remove (Agent a) {
		return remove (a, getCellIndex (a.renderer.bounds.center));
	}

	public void move (Agent a, Vector2 from, Vector2 to) {
		remove (a, from);
		add (a, to);
	}

	// This gets the adjacent agents within a given radius of Agent A
	// If looking for adjacent node sensor, it will be in Map.cs
	public List<Agent> getNear (Agent a, float radius, Type agentType = null) {
		List<Agent> near = new List<Agent> ();

		agentType = agentType == null? typeof(Agent): agentType;

		float myRadius = a.getRadius();
		radius += myRadius;

		Vector2 lower = getCellIndex (a.renderer.bounds.center - new Vector3 (radius+ maxAgentRadius, radius+ maxAgentRadius, 0));
		Vector2 upper = getCellIndex (a.renderer.bounds.center + new Vector3 (radius+ maxAgentRadius, radius+ maxAgentRadius, 0));

		for (int i = (int)lower.y; i <= (int)upper.y; ++i) {
			for (int j = (int)lower.x; j <= (int)upper.x; ++j) {

				if (!inBounds(j,i))
					continue;

				for (int k = 0; k < grid[i,j].Count; ++k) {

					Agent b = grid[i,j][k];
					if (b.Equals(a) || !agentType.IsAssignableFrom(b.GetType())) continue;

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


	// Returns a list of the Icicle Penguins within the drawn selection box
	public List<IciclePenguins> multiSelectPenguins(Vector3 startBound, Vector3 endBound){
		List<IciclePenguins> selected = new List<IciclePenguins> ();

		Vector3 center = new Vector3 ((startBound.x+endBound.x)/2.0f, (startBound.y+endBound.y)/2.0f, 0.0f);
		//size.x is the width, size.y is the height and size.z is the depth of the box.
		Vector3 size = new Vector3 (Math.Abs(startBound.x-endBound.x), Math.Abs(startBound.y-endBound.y), 0.0f);

		//create box
		Bounds b = new Bounds (center, size);

		//loop through grid to see what agents are within the box, and on the box's intersecting lines
		for (int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++){
				foreach(Agent a in grid[i,j]){
					if(a.GetType().Equals(typeof(IciclePenguins)) && b.Intersects(a.renderer.bounds)){
						selected.Add((IciclePenguins) a);
					}
				}
			}
		}

		return selected;
	}


	// Flag: Has Dispose already been called? 
		bool disposed = false;
	
	// Public implementation of Dispose pattern callable by consumers. 
	public void Dispose()
	{ 
		Dispose(true);
		GC.SuppressFinalize(this);           
	}
	
	// Protected implementation of Dispose pattern. 
	protected virtual void Dispose(bool disposing)
	{
		if (disposed)
			return; 
		
		if (disposing) {
			removeAllAgents();
		}
		
		disposed = true;
	}
}
