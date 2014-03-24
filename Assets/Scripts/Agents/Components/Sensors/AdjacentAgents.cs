using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AdjacentAgents : Sensor {
	
	public List<Agent> near;

	public Grid grid;
	public float adjAgentsRadius;
	public float adjAgentsTotalRadius;

	public float adjAgentsRadiusCameraSpace;
	public float adjAgentsTotalRadiusCameraSpace;

	public AdjacentAgents(Agent me, float radius, Grid grid) : base(me)  {

		this.adjAgentsRadius = radius;
		adjAgentsTotalRadius = radius + me.getRadius();

		this.grid = grid;
		adjAgentsRadiusCameraSpace = DebugRenderer.worldToCameraLength(radius);
		adjAgentsTotalRadiusCameraSpace = adjAgentsRadiusCameraSpace + me.getRadiusCameraSpace();
	}

	public override void calculate() {
		near = grid.getNear(me, adjAgentsRadius);

		debugInformation = near.Count +" agents found within adjacent agent sensor.\n";
	}

	//Draws adjacent agent circle based on a specified radius range
	public override void drawDebugInformation() {
		DebugRenderer.drawCircle(me.getCenterCameraSpace(), adjAgentsTotalRadiusCameraSpace);
	}
}
