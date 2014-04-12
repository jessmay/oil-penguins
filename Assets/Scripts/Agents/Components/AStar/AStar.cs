using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar {

	Map map;

	public Vector2 source;
	public Vector2 target;

	public Vector2 sourceCell;
	public Vector2 targetCell;

	public List<Vector2> currPath;
	public bool hasPath;

	public AStar(Map m){
		map = m;
	}

	public void setSource(Vector2 s){
		source = s;
		sourceCell = map.getCellIndex (s);
	}

	public void setTarget(Vector2 t){
		target = t;
		targetCell = map.getCellIndex (t);
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
					if(!map.inBounds(new Vector2(x, y)) || !map.canMove[x,y])
						continue;
					
					//corner case:if node to move to is diagonal, but perp nodes are nonmoveable, can't go there
					if(Mathf.Abs(i) == Mathf.Abs(j) && (!map.canMove[x,(int)currNode.y] || !map.canMove[(int)currNode.x,y])){
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

}
