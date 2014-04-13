using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar {

	Agent agent;

	public Vector2 source;
	public Vector2 target;

	public Vector2 sourceCell;
	public Vector2 targetCell;

	public List<Vector2> currPath;
	public bool hasPath;

	public AStar(Agent a){
		agent = a;
	}

	public void setSource(Vector2 s){
		source = s;
		sourceCell = agent.map.getCellIndex (s);
	}

	public void setTarget(Vector2 t){
		target = t;
		targetCell = agent.map.getCellIndex (t);
	}

	// Not using a priority queue, using a hash table, so have to find the node with the min f value each round
	private Vector2 findMin(Hashtable l, List<Vector2> curr){
		float min = -1.0f;
		Vector2 minVec = new Vector2 (0, 0);
		
		foreach (DictionaryEntry d in l) {
			if((((float)d.Value) < min || min == -1) && curr.Contains((Vector2)d.Key)){
				min = (float)d.Value;
				minVec = (Vector2)d.Key;
			}
		}
		
		return minVec;
	}
	
	// A* algorithm
	// Uses map cells to detect where walls are
	// If there is a path, it will be reconstructed at the end of this function
	// and it will contain the locations of nodes in Map Cell space to follow
	// Source node and target node are in world coords
	public bool aStar(){
		
		List<Vector2> visited = new List<Vector2> ();
		List<Vector2> curr = new List<Vector2> ();
		curr.Add (sourceCell);
		Hashtable from = new Hashtable ();
		
		Hashtable gVals = new Hashtable ();
		gVals.Add (sourceCell, 0.0f);
		Hashtable fVals = new Hashtable ();
		fVals.Add (sourceCell, ((float)gVals [sourceCell]) + Vector2.Distance (sourceCell, targetCell));
		
		while (curr.Count != 0) {
			
			Vector2 currNode = findMin(fVals, curr);
			
			// Arrived at the target? Great! Make the path list.
			if(currNode == targetCell){
				//reconstruct path with the from list
				currPath = new List<Vector2>();
				currPath = createPath(from, currNode);
				pathSmoothQuick();//TODO test if want to use quick or precise
				hasPath = true;
				return true; 
			}
			
			curr.Remove(currNode);
			visited.Add(currNode);
			
			//for each neighbor node, add to curr if unvisited and calculate f and g values
			for(int i = -1; i < 2; i++){
				for(int j = -1; j < 2; j++){
					
					int x = (int)currNode.x+i;
					int y = (int)currNode.y+j;
					
					//currnode not inbounds, or not moveable to
					if(!agent.map.inBounds(new Vector2(x, y)) || !agent.map.canMove[x,y])
						continue;
					
					//corner case:if node to move to is diagonal, but perp nodes are nonmoveable, can't go there
					if(Mathf.Abs(i) == Mathf.Abs(j) && (!agent.map.canMove[x,(int)currNode.y] || !agent.map.canMove[(int)currNode.x,y])){
						continue;
					}
					
					Vector2 neighbor = new Vector2(x, y);
					if(visited.Contains(neighbor)){//also covers 0,0 case
						continue;
					}
					
					float tempG = (float)gVals[currNode] + Vector2.Distance(currNode, neighbor);
					
					if(!curr.Contains(neighbor) || tempG < (float)gVals[currNode]){
						
						from.Add(neighbor, currNode);
						gVals.Add(neighbor, tempG);
						fVals.Add(neighbor, tempG + Vector2.Distance(neighbor, targetCell));
						
						if(!curr.Contains(neighbor))
							curr.Add(neighbor);
					}
				}
			}
		}
		return false;
	}
	
	
	// If aStar() finds a path, this function creates a List for that path
	// that the agent can then access
	private List<Vector2> createPath(Hashtable path,  Vector2 node){
		
		if (path.Contains (node)) {
			List<Vector2> p = new List<Vector2>();
			p = createPath(path, (Vector2)path[node]);
			p.Add(node);
			return p;
		}
		else{
			List<Vector2> p = new List<Vector2>();
			p.Add(node);
			return p;
		}
		
	}

	private void pathSmoothQuick(){
		List<Vector2> newPath = new List<Vector2> ();
		int index1 = 0;
		int index2 = 1;

		newPath.Add (currPath [index1]);

		while (index2 < currPath.Count) {
			if(canWalkBetween(currPath[index1], currPath[index2])){
				//don't put the item at index2 in the path, look at next item
				index2++;
			}
			else{
				newPath.Add(currPath[index2]);
				index1 = index2;
				index2++;
			}
		}

		currPath = newPath;
	}

	private void pathSmoothPrecise(){
		List<Vector2> newPath = new List<Vector2> ();
		int index1 = 0;
		int index2 = 0;

		newPath.Add (currPath [index1]);

		while (index1 < currPath.Count-1) {
			index2 = index1+1;

			while(index2 < currPath.Count-1){

				if(canWalkBetween(currPath[index1], currPath[index2])){
					index1 = index2-1;
				}
				else{
					newPath.Add(currPath[index2]);
					index2++;
				}
			}

			index1++;
		}
	}

	// Checks if an agent can walk between two given points
	// This takes the agents radius into account and uses ray-casting
	// Points should be given in Map coordinates, they will be converted to world
	private bool canWalkBetween(Vector2 startPoint, Vector2 endPoint){

		//Converts given map coordinates to world coordinates
		Vector2 startWorld = agent.map.cellIndexToWorld (startPoint);
		Vector2 endWorld = agent.map.cellIndexToWorld (endPoint);

		//Get the vector between the 2 given points
		Vector3 pointPath = endWorld - startWorld;

		//Find the vector in the direction of the radius
		Vector3 radDirection = Vector3.Cross (pointPath, agent.transform.forward);

		//Normalize the vector
		Vector3.Normalize (radDirection);

		//Multiply by the radius of the agent
		radDirection = radDirection * agent.getRadius ();

		//Get the points we will ray cast from
		Vector2 rightStart = startWorld + (Vector2)radDirection;
		Vector2 leftStart = startWorld - (Vector2)radDirection;

		//Raycast to see if there are any collisions before the end point
		RaycastHit2D ray1 = Physics2D.Raycast (rightStart, pointPath, pointPath.magnitude, (1 << LayerMask.NameToLayer("Wall")));
		RaycastHit2D ray2 = Physics2D.Raycast (leftStart, pointPath, pointPath.magnitude, (1 << LayerMask.NameToLayer("Wall")));

		return (ray1.collider == null && ray2.collider == null);
	}

}
