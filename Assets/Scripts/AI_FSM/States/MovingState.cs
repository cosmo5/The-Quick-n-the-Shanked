using UnityEngine;
using System.Collections;

public class MovingState : IState<Inmate_Entity>
{
    static readonly MovingState instance = new MovingState();

    public static MovingState Instance
    {
        get { return instance; }
    }
    //Entering The State
    public void Enter(Inmate_Entity e)
    {
        Debug.Log("Entering Move State");
    }
    //Exiting The State
    public void Run(Inmate_Entity e)
    {
        Debug.Log("Running Move State");

    }

    public void Exit(Inmate_Entity e)
    {
        Debug.Log("Exiting MoveState");
    }
}
