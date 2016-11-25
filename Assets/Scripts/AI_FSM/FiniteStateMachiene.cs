using UnityEngine;
using System.Collections;

public class FiniteStateMachiene<T> {

    private T owner;

    private IState<T> currentState;
    private IState<T> previousState;
    private IState<T> worldState;
    // Use this for initialization
    void Awake () {
        currentState = null;
        previousState = null;
        worldState = null;
	}
	public void Update()
    {

        if (currentState != null)
        {
            currentState.Run(owner);
        }
        if (worldState != null)
        {
            worldState.Run(owner);
        }
    }
  
	public void Init(T _owner, IState<T> initalState)
    {
        owner = _owner;
        ChangeState(initalState);
    }

    public void ChangeState(IState<T> state)
    {
        previousState = currentState;

        if (currentState != null)
            currentState.Exit(owner);
        currentState = state;
        if (currentState!=null)
            currentState.Enter(owner);
    }

    //Possible revert to prev
}
