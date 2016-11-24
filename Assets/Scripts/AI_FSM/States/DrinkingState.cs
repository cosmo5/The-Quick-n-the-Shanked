using UnityEngine;
using System.Collections;

public class DrinkingState : IState<Inmate_Entity>
{
    static readonly DrinkingState instance = new DrinkingState();
    //Entering The State
    public void Enter(Inmate_Entity e)
    {
        Debug.Log("Entering Drinking State");
    }
    //Exiting The State
    public void Run(Inmate_Entity e)
    {
        Debug.Log("Running Drink State");
    }

    public void Exit(Inmate_Entity e)
    {
        Debug.Log("Exiting Drink State");

    }
}
