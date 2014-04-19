using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStar {

	Agent agent;

	public Vector2 source;
	public Vector2 target;

	public Vector2 sourceCell;
	public Vector2 targetCell;

	public List<Vector2> prevPath;
	public List<Vector2> currPath;
	public bool hasPath;

	public bool findTarget;
	public Vector2 currGoal;
	protected int pathIndex;

	public AStar(Agent a){
		agent = a;
		findTarget = false;
		currGoal = agent.transform.position;
		hasPath = false;
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
	// and it will contain the locations of nodes in World space to follow; the nodes will be the centers of open map cells
	// Source node and target node are in world coords
	public bool aStar(){
		
		List<Vector2> visited = new List<Vector2> ();
		List<Vector2> curr = new List<Vector2> ();
		curr.Add (sourceCell);
		Hashtable from = new Hashtable ();
		
		Hashtable gVals = new Hashtable ();
		gVals.Add (sourceCell, 0.0f);
		Hashtable fVals = new Hashtable ();
		fVals.Add (sourceCell, Vector2.Distance (sourceCell, targetCell));
		
		while (curr.Count != 0) {
			
			Vector2 currNode = findMin(fVals, curr);
			
			// Arrived at the target? Great! Make the path list.
			if(currNode == targetCell){
				//reconstruct path with the from list
				currPath = new List<Vector2>();
				currPath = createPath(from, currNode);
				
				currPath.Insert(0, source);
                currPath.Add(target);

				prevPath = currPath;

				/*Debug.Log ("PREVPATH");
				for(int i = 0; i < prevPath.Count;i++)
					Debug.Log(prevPath[i].ToString());*/

				// TODO FIX THE PATH SMOOTHS
				//pathSmoothQuick();//TODO test if want to use quick or precise

				/*Debug.Log("currpath");
				for(int i = 0; i < currPath.Count;i++)
					Debug.Log(currPath[i].ToString());*/
                

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
		
		if (path.Contains(node)) {
			List<Vector2> p = new List<Vector2>();
			p = createPath(path, (Vector2)path[node]);
			p.Add(agent.map.cellIndexToWorld(node));
			return p;
		}
		else{
			List<Vector2> p = new List<Vector2>();
			p.Add(agent.map.cellIndexToWorld(node));
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
	// Points should be given in world coordinates
	private bool canWalkBetween(Vector2 startPoint, Vector2 endPoint){

		//Get the vector between the 2 given points
		Vector3 pointPath = endPoint - startPoint;

		//Find the vector in the direction of the radius
		Vector3 radDirection = Vector3.Cross (pointPath, agent.transform.forward);

		//Normalize the vector
		Vector3.Normalize (radDirection);

		//Multiply by the radius of the agent
		radDirection = radDirection * agent.getRadius ();

		//Get the points we will ray cast from
		Vector2 rightStart = startPoint + (Vector2)radDirection;
		Vector2 leftStart = startPoint - (Vector2)radDirection;

		//Raycast to see if there are any collisions before the end point
		RaycastHit2D ray1 = Physics2D.Raycast (rightStart, pointPath, pointPath.magnitude, (1 << LayerMask.NameToLayer("Wall")));
		RaycastHit2D ray2 = Physics2D.Raycast (leftStart, pointPath, pointPath.magnitude, (1 << LayerMask.NameToLayer("Wall")));

		return (ray1.collider == null && ray2.collider == null);
	}


	public void aStarUpdate(){
		
		//Seeks along A* path if findTarget is true
		if(findTarget){
			
			//If the player is at the target, no more need to find the target
			if(agent.distanceBetweenPoint(target) <= (.5 * agent.transform.localScale.x)){//(Vector2)agent.transform.position == target
				pathIndex = 0;
				findTarget = false;
				hasPath = false;

				//Debug.Log("FALSE");
			}
			//Otherwise seek towards the current goal location in the aStar path
			else {	
				//If in the cell index of current goal and not on target, close enough, check next location to seek
				if (agent.distanceBetweenPoint(currGoal) <= (.5 * agent.transform.localScale.x) && pathIndex < currPath.Count-1)
				{
					pathIndex++;
					currGoal = new Vector2();
					currGoal = (Vector2)currPath[pathIndex];
					agent.seek (currGoal);
				}
				//If not in the cell index of current goal, keep seeking to that current goal
				else if (agent.distanceBetweenPoint(currGoal) > (.5 * agent.transform.localScale.x))
				{
					agent.seek(currGoal);
				}
			}
		}
	}


}
