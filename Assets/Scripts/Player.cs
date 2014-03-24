/*
Jessica May
Joshua Linge
Player.cs
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : Agent {

	// Use this for initialization
	new void Start () {
		base.Start();
		feelerLength = (int)(radius*3);
		adjRadius = (int)(radius*3);
		getPieSliceAngles ();
	}
	
	// Update is called once per frame
	void Update () {

		//display feelers
		if (Input.GetKeyDown(KeyCode.Z)) {
			dispFeelers = !dispFeelers;
		}

		//display pie slices
		if(Input.GetKeyDown(KeyCode.X)){
			dispSlices = !dispSlices;
		}

		//display adjacent agents
		if(Input.GetKeyDown(KeyCode.C)){
			dispAdjAgent = !dispAdjAgent;
		}

		//Display debug text
		if(Input.GetKeyDown(KeyCode.Q)) {
			debug = !debug;
		}

		//if turning CCW, increment heading
		if (Input.GetKey(KeyCode.LeftArrow)) {
			lookLeft();
		}

		//If turning CW, decrement heading
		if (Input.GetKey(KeyCode.RightArrow)) {
			lookRight();
		}

		//If moving forward
		if (Input.GetKey(KeyCode.UpArrow)) {
			moveForward();
		}

		//If moving backward
		if (Input.GetKey(KeyCode.DownArrow)) {
			moveBackward();
		}

		//Assignment 2
		// If the source node or target node change, the aStar path needs to be updated
		bool sourceChange = false;
		bool targetChange = false;

		//Assignment 2
		//Designate source location; shift + left click
		if (Input.GetKey (KeyCode.LeftShift) && Input.GetMouseButtonDown (0)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			//if new source location, sets values of source to mouse position
			if((int)pos.x != source.x || (int)pos.y != source.y){
				source = pos;
				sourceCell = background.grid.getCellIndex(source);
				sourceChange = true;
			}
		}

		//Assignment 2
		//Designate target location; left control+right click
		if(Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(1)){
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			//if new target location, sets values of target to mouse position
			if((int)pos.x != target.x || (int)pos.y != target.y){
				target = pos;
				targetCell = background.grid.getCellIndex(target);
				targetChange = true;
			}
			
		}

		//Assignment 2
		//if there has been a change in the findTarget value during this call of update
		bool findChange = false;
		//tells the player whether or not to perform A* and seek the target
		if(Input.GetKeyDown(KeyCode.F1)){
			findTarget = !findTarget;
			findChange = true;
		}

		// Assignment 2
		// tells the player whether or not to display the source node for debugging purposes
		if (Input.GetKeyDown (KeyCode.U)) {
			drawSource = !drawSource;
		}

		//Assignment 2
		// tells the player whether or not to display the target node for debugging purposes
		if (Input.GetKeyDown (KeyCode.I)) {
            drawTarget = !drawTarget;
        }
        
		//Assignment 2
		// tells the player whether or not to display the nav graph for debugging purposes
        if (Input.GetKeyDown (KeyCode.O)) {
            drawNodes = !drawNodes;
        }
        
		//Assignment 2
		// tells the player whether or not to display the A* path for debugging purposes
        if (Input.GetKeyDown (KeyCode.P)) {
            drawPath = !drawPath;
        }

		//Get length of feelers
		getLengthOfFeelers(feelerLength, numFeelers, viewAngle);

		//Get list of nearest agents
		near = background.grid.getNear (this, adjRadius);

		//Get agents in pie slice angles
		findPieSlices ();

		//Assignment 2
		//Get list of close nodes, gets the closest, seeks, and calculates astar path
		if((findChange || targetChange || sourceChange) && findTarget && source != target){
			//Gets list of close nodes within one cell of the agent
			closeNodes = new List<Vector2>();
			closeNodes = background.map.getNearNodes(this);

			// Gets the closest node from the list of close nodes and sets it as the source node and the currGoal
			currGoal = new Vector2();
			currGoal = closestUnobstructed();
			source = new Vector2();
			source = background.map.cellIndexToWorld(currGoal);

			//gets aStar path if there is one, otherwise turns target seeking off
			if(!aStar(source)){
				findTarget = false;
			}

			pathIndex = 0;
		}

		//Assignment 2
		//Seeks along A* path if findTarget is true
		if(findTarget){

			//If the player is at the target, no more need to find the target
			if(background.map.getCellIndex(renderer.bounds.center) == background.map.getCellIndex(target)){
				pathIndex = 0;
				findTarget = false;
			}
			//Otherwise seek towards the current goal location in the aStar path
			else {

				//If in the cell index of current goal and not on target, close enough, check next location to seek
				if (background.map.getCellIndex(renderer.bounds.center) == currGoal && pathIndex < currPath.Count-1)
	            {
					pathIndex++;
					currGoal = new Vector2();
					currGoal = (Vector2)currPath[pathIndex];
					seek ();
					moveForward();
				}
				//If not in the cell index of current goal, keep seeking to that current goal
				else if (background.map.getCellIndex(renderer.bounds.center) != currGoal)
				{
					seek();
					moveForward();
	            }
			}
        }

		//Move player based off velocity and heading
        Move ();
    }
    
	//Calls the DrawOnGUI() method from the agent class in order to draw debugging output on the screen
    void OnGUI(){
        DrawOnGUI ();
    }
    
   
}