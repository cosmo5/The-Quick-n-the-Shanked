//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;

////This class is the base class for the transition tables, it handles the adding of states and getting the current state.
//abstract public class StateTransitionTable<T> {

//	protected Dictionary<string, IState<T> EventTable = new Dictionary<string, IState>();

//    //Set State Function
//    public void SetState(string evt, IState state)
//    {
//        EventTable.Add(evt, state);
//    }
//    public IState GetState(string evt)
//    {
//        IState i = null;

//        try
//        {
//            i = EventTable[evt];
//        }
//        catch (KeyNotFoundException)
//        {
//            MonoBehaviour.print("State Not Found");
//            return null;
            
//        }

//        return i;
//    }


//}
