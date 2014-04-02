using UnityEngine;
using System.Collections;

public class Human : TestableAgent {
	
	public override float getTurnStep() { return turnStep; }
	public override float getMoveStep() { return 10.0f * transform.localScale.x;}

	// Use this for initialization
	protected override void initializeAgent () {
		base.initializeAgent();
	}


	//Get ICE machine's location from map.
	public override Vector2 getTarget() {
		return map.cellIndexToWorld(map.ICEMachineLocation);
	}
	
	//Nothing to deconstruct
	protected override void destroyAgent() {}
	
	
	// Update is called once per frame
	protected override void updateAgent () {
		base.updateAgent();

	}
	




//	
//	//Returns when the agent is controllable.
//	protected override bool isControllable(){
//		return false;
//	}
//	
//	
//	//Draw debug information to the screen.
//	protected override void DrawDebugInformation(){
//		
//		//Draw current target if target is enabled.
////		if (targetsEnabled){
////			Vector3 t = getCurrentTargetVector();
////			Vector3 cst = DebugRenderer.currentCamera.WorldToScreenPoint(t);
////			cst.y = Screen.height - cst.y;
////			
////			DebugRenderer.drawCircle(cst, DebugRenderer.worldToCameraLength(1));
////		}
//		
//		//Draw sensors to the screen.
//		{
//			// Draw feelers
//			feelers.drawSensor();
//			
//			//Draw circle for nearest agents
//			adjAgents.drawSensor();
//			
//			//Draw pie slices
//			pieSlices.drawSensor();
//		}
//		
//		//Draw debug text to the screen
//		{
//			
//			//Get agent information
//			string debugText = "Agent Id: "+ gameObject.GetInstanceID() +"\n";
//			debugText += "Coordinates: " +"("+renderer.bounds.center.x +", "+ renderer.bounds.center.y+")" +"\n";
//			debugText += "Heading: " + heading + ".\n\n";
//			
//			
//			debugText += "Senses:\n";
//			for (int currSense = 0; senses != null && currSense < senses.Length; ++currSense) {
//				debugText += senses[currSense] +"\n";
//			}
//			debugText += "\n";
//			
//			
//			debugText += "Thoughts:\n";
//			for (int currThought = 0; thoughts != null && currThought < thoughts.Length; ++currThought) {
//				debugText += thoughts[currThought] +"\n";
//			}
//			
//			debugText += "\n";
//
//			debugText += brain.getDebugInformation() + "\n";
//
//			//Get sensor information
//			debugText += feelers.getDebugInformation()+ "\n";
//			debugText += adjAgents.getDebugInformation()+ "\n";
//			debugText += pieSlices.getDebugInformation()+ "\n";
//			
//			GUI.color = Color.black;
//			GUI.Label(new Rect(0, 0, 300, 800), debugText);
//		}
//	}
}
