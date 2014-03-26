using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class GUIFSM {

	//The pause menu for this GUI finite state machine.
	public PauseMenu pauseMenu {get; private set;}

	protected GUIState currentState;
	protected GUIState previousState;

	//A map to keep track off all states.
	private Dictionary<Type, GUIState> states; 

	//Initialize this finite state machine.
	public abstract void initialize();

	//Get the default state for this finite state machine.
	public abstract Type getDefaultState();
	
	public GUIFSM(PauseMenu pauseMenu) {

		this.pauseMenu = pauseMenu;

		states = new Dictionary<Type, GUIState>();
		//initialize();
	}
	
	public void update() {
		
		if (currentState != null)
			currentState.updateGraphics();
	}

	public void display() {

		if (currentState != null)
			currentState.displayGraphics();
	}

	//Add a state to the state list.
	public void addState<T> (T state) where T : GUIState {

		states.Add(state.GetType(), state);
	}

	//Get a state from the state list.
	public GUIState getState (Type state) {

		if (!typeof(GUIState).IsAssignableFrom(state)) {
			throw new Exception("Type "+state +" is not a GUIState.");
		}
		return states[state];
	}

	//Change to the given state.
	public void changeState(Type stateType, int code = GUIState.DEFAULT_CODE) {
		
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

