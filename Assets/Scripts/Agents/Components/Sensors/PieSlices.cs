using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieSlices : Sensor {

	public int numSlices;
	public int pieAngle;
	public float sliceAngle;
	public List<Agent>[] agentsWithinSlices;

	public AdjacentAgents adjAgents;

	public PieSlices (Agent me, AdjacentAgents adjAgents, int numSlices = 4, int pieAngle = 360) : base(me) {

		this.adjAgents = adjAgents;
		this.numSlices = numSlices;
		this.pieAngle = pieAngle;

		sliceAngle = pieAngle/(float)numSlices;

		agentsWithinSlices = new List<Agent>[numSlices];
	}

	public override void calculate(){

		List<Agent> near = adjAgents.near;

		float heading = me.getHeading();
		Vector2 headingVec = new Vector2(-Mathf.Sin(Mathf.Deg2Rad*heading), Mathf.Cos(Mathf.Deg2Rad*heading));

		//Reset list
		for(int currSlice = 0; currSlice < numSlices; currSlice++){
			agentsWithinSlices[currSlice] = new List<Agent>();
		}
		
		//Loop through each agent within the adjacent agent sensor
		for(int currAgent = 0; currAgent < near.Count; currAgent++){

			//Get vector from player to agent.
			Vector2 playerToAgent = near[currAgent].renderer.bounds.center - me.gameObject.renderer.bounds.center;

			//Get angle between heading and playerToAgent
			float agentAngle = Mathf.Acos(Vector2.Dot(headingVec.normalized, playerToAgent.normalized));
			agentAngle *= Mathf.Rad2Deg;
			if(Vector3.Cross(headingVec, playerToAgent).z < 0){
				agentAngle = 360-agentAngle;
			}

			//Loop through angles until the current angle is larger. The number of iterations equals the pie slice index.
			int sliceIndex;
			for (sliceIndex = 0; agentAngle > (0.5f + sliceIndex)*sliceAngle && sliceIndex < numSlices; ++sliceIndex);

			//Add agent to pie slice
			agentsWithinSlices[sliceIndex%numSlices].Add(near[currAgent]);
		}

		debugInformation = "";
		for(int currSlice = 0; currSlice < numSlices; currSlice++){
			debugInformation += "PieSlice["+currSlice+"]: "+ agentsWithinSlices[currSlice].Count +"\n";
		}
	}
	
	//Draw pie slices
	public override void drawDebugInformation () {
		
		float heading = me.getHeading();
		float width = DebugRenderer.lineWidth;
		float length = adjAgents.adjAgentsRadiusCameraSpace;
		Vector2 center = me.getCenterCameraSpace();
		float radius = me.getRadiusCameraSpace();

		Color color = Color.blue;
		color.a = .5f;
		
		for (int i = 0; i < numSlices; i++) {
			DebugRenderer.drawBox (center.x-width/2, center.y+radius, width, length, -heading+(0.5f + i)*sliceAngle, center, color);
		}
	}
}
