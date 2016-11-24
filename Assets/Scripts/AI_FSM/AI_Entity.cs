using UnityEngine;
using System.Collections;

abstract public class AI_Entity : MonoBehaviour {

    //Base AI class this class handles the updating of the AI states.
    protected static StateTransitionTable transTable = null;

    protected IState currState;
	
    public void UpdateState()
    {
        //Check the state isnt null
        if (currState != null)
            currState.Run(this);
        else
            Debug.Log("Current State Is Null");
    }

    public object Event
    {
        //Sets the new Event
        set
        {
            //if value is null then exit state and return
            if (value == null)
            {
                currState.Exit(this);
                currState = null;
                return;
            }
            //create new state from Transitino Table
            IState newState = transTable.GetState(value);
            if (newState != null)
            {
                //if current state isnt null exit the state
                if (currState != null)
                    currState.Exit(this);
                //set new state
                currState = newState;
                currState.Enter(this);
         
            }
        }
    }
    
}
