using UnityEngine;
using System.Collections;

abstract public class AI_Entity<T> : MonoBehaviour {

    //Base AI class this class handles the updating of the AI states.
   // protected static StateTransitionTable<T> transTable = null;
    private T owner;
   
    protected int ID;

 
}
