using UnityEngine;
using System.Collections;

public class EatingState : IState<Inmate_Entity>
{
    static readonly EatingState instance = new EatingState();
    //Entering The State
    public void Enter(Inmate_Entity e)
    {
        Debug.Log("Entering Eat State");

    }
    //Exiting The State
    public void Run(Inmate_Entity e)
    {
        Debug.Log("Running Eating State");

    }

    public void Exit(Inmate_Entity e)
    {
        Debug.Log("Exiting Eating State");

    }
}

