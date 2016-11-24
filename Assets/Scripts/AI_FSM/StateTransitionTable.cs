using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

//This class is the base class for the transition tables, it handles the adding of states and getting the current state.
abstract public class StateTransitionTable {

	protected Dictionary<object, IState> transitionTable = new Dictionary<object, IState>();

    //Set State Function
    public void SetState(object evt, IState state)
    {
        transitionTable.Add(evt, state);
    }
    public IState GetState(object evt)
    {
        IState i = null;

        try
        {
            i = transitionTable[evt];
        }
        catch (KeyNotFoundException)
        {
            MonoBehaviour.print("State Not Found");
            return null;
            
        }

        return i;
    }


}
