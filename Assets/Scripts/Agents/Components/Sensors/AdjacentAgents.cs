using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AdjacentAgents : Sensor {
	
	public List<Agent> near;

	public Grid grid;
	public float adjAgentsRadius;
	public float adjAgentsTotalRadius;

	public float adjAgentsRadiusCameraSpace;
	public float adjAgentsTotalRadiusCameraSpace;

	private Type agentType;

	public AdjacentAgents(Agent me, float radius, Grid grid, Type agentType = null) : base(me)  {

		this.adjAgentsRadius = radius;
		adjAgentsTotalRadius = radius + me.getRadius();

		this.grid = grid;
		adjAgentsRadiusCameraSpace = DebugRenderer.worldToCameraLength(radius);
		adjAgentsTotalRadiusCameraSpace = adjAgentsRadiusCameraSpace + me.getRadiusCameraSpace();

		this.agentType = agentType;
	}

	public override void calculate() {
		near = grid.getNear(me, adjAgentsRadius, agentType);

		debugInformation = near.Count +" agents found within adjacent agent sensor.\n";
	}

	//Draws adjacent agent circle based on a specified radius range
	public override void drawDebugInformation() {

		Color color = Color.yellow;
		color.a = .5f;

		float width = DebugRenderer.worldToCameraLength(adjAgentsTotalRadius);

		DebugRenderer.drawCircle(me.getCenterCameraSpace(), width, color);
	}
}
