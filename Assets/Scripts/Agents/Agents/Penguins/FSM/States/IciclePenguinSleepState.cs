using UnityEngine;
using System.Collections;

public class IciclePenguinSleepState : State {

	private IciclePenguinFSM IPfsm;
	public float sleepTimer;
	public float sleepTime = 20;
	public int timesSleep = 0;
	
	public IciclePenguinSleepState (IciclePenguinFSM fsm) : base(fsm) {
		IPfsm = fsm;
	}
	
	public override string getName () {
		return "Sleep State";
	}
	
	public override void enter(){
		//Updates the penguin sprite
		IPfsm.penguin.GetComponent<SpriteRenderer>().sprite = IPfsm.penguin.penguinSprites [2];

		//cannot select penguin when sleeping
		IPfsm.penguin.selectable = false;

		//Set sleep timer
		sleepTimer = Time.time + sleepTime;

		// Increment times this penguin has fallen asleep
		timesSleep++;

		// Adjust sleepTime
		sleepTime += 5;

		//Tell Game Manager penguin is asleep
		IPfsm.penguin.gameMap.sleepingPenguins++;
	}
	
	public override void exit(){
		//Change back to selectable
		IPfsm.penguin.selectable = true;

		//Tell Game Manager penguin is awake
		IPfsm.penguin.gameMap.sleepingPenguins--;
	}
	
	//check if need to move to new state
	public override void update(){
		//Changes state if needed
		updateState ();
	}
	
	//Sees if there needs to be a state change
	public void updateState(){
		//if sleep timer runs out, wake up and go to Chillin State
		if(Time.time >= sleepTimer){
			//Change state to chillin
			finiteStateMachine.changeState(typeof(IciclePenguinChillinState));
		}
	}
	
	protected override bool isValidStatus(int statusCode){
		return (statusCode == DEFAULT_CODE);
	}
}
