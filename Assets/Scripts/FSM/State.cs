using UnityEngine;
using System;
using System.Collections;

public abstract class State {

	protected FiniteStateMachine finiteStateMachine;
	
	public abstract string getName();
	public abstract void enter();
	public abstract void exit();
	public abstract void update();

	public State(FiniteStateMachine fsm) {
		finiteStateMachine = fsm;
	}
	
	//Used to give states extra information
	//States within states.
	public int statusCode {get; private set;}
	public const int DEFAULT_CODE = 0;
	
	protected abstract bool isValidStatus(int statusCode);
	
	public void setStatusCode(int code) {
		
		if(!isValidStatus(code)) {
			
			throw new Exception("Invalid status of "+ code +" when entering into " +GetType());
		}
		
		statusCode = code;
	}
}

