using UnityEngine;
using System.Collections;

abstract public class AI_Entity<T> : MonoBehaviour {

    //Base AI class this class handles the updating of the AI states.
   // protected static StateTransitionTable<T> transTable = null;
    private T owner;
    protected IState<T> currState;
    protected int ID;
    public void Update()
    {
        //Check the state isnt null
        if (currState != null)
            currState.Run(owner);
        else
            Debug.Log("Current State Is Null");
    }

    //public string Event
    //{
    //    //Sets the new Event
    //    set
    //    {
    //        //if value is null then exit state and return
    //        if (value == null)
    //        {
    //            currState.Exit(owner);
    //            currState = null;
    //            return;
    //        }
    //        //create new state from Transitino Table
    //        IState<T> newState = transTable.GetState(value);
    //        if (newState != null)
    //        {
    //            //if current state isnt null exit the state
    //            if (currState != null)
    //                currState.Exit(owner);
    //            //set new state
    //            currState = newState;
    //            currState.Enter(owner);
         
    //        }
    //    }
   // }
    
}
