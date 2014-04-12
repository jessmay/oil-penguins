using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class FiniteStateMachine {

	public State currentState {get; private set;}
	protected State previousState;

	//A map to keep track off all states.
	private Dictionary<Type, State> states; 
	
	//Initialize this finite state machine.
	protected abstract void initialize();

	//Get the default state for this finite state machine.
	protected abstract Type getDefaultState();

	public FiniteStateMachine() {

		states = new Dictionary<Type, State>();
		initialize();
	}

	public void update() {

		if (currentState != null)
			currentState.update();
	}
	
	//Add a state to the state list.
	public void addState<T> (T state) where T : State {
		
		states.Add(state.GetType(), state);
	}

	//Get a state from the state list.
	public State getState (Type state) {
		
		if (!typeof(State).IsAssignableFrom(state)) {
			throw new Exception("Type "+state +" is not a State.");
		}
		return states[state];
	}

	//Change to the given state.
	public void changeState(Type stateType, int code = State.DEFAULT_CODE) {
		
		previousState = currentState;
		
		if(previousState != null)
			previousState.exit();
		
		if (stateType == null)
			stateType = getDefaultState();
		
		currentState = getState(stateType);
		
		currentState.setStatusCode(code);
		currentState.enter();
	}
	
	//Change to the previous state
	public void changeToPreviousState() {
		
		changeState(previousState.GetType(), previousState.statusCode);
	}
}

