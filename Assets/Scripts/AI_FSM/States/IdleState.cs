using UnityEngine;
using System.Collections;

public class IdleState : IState<Inmate_Entity>
{
    static readonly IdleState instance = new IdleState();
    public static IdleState Instance
    {
        get { return instance; }
    }
    //Entering The State
    public void Enter(Inmate_Entity e)
    {
        Debug.Log("Entering Idle State");
    }
    //Exiting The State
    public void Run(Inmate_Entity e)
    {
        Debug.Log("Running Idle State");
    }

    public void Exit(Inmate_Entity e)
    {
        Debug.Log("Exiting Idle State");
    }
}
