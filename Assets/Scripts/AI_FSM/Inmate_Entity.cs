using UnityEngine;
using System.Collections;
/// <summary>
/// All the different states and their functionality
/// </summary>
public class Inmate_Entity : AI_Entity
{
   
    static Inmate_Entity()
    {
        transTable = new InmateTransitions();
    }
}
enum Events
{
    Moving,
    Drinking,
    Eating
}

public class InmateTransitions: StateTransitionTable
{
    public InmateTransitions()
    {
        transitionTable.Add(Events.Moving, new InmateMovingState());
        transitionTable.Add(Events.Eating, new InmateEatingState());
        transitionTable.Add(Events.Drinking, new InmateDrinkingState());
    }
}
public class InmateMovingState : IState
{
    //Entering The State
    public void Enter(AI_Entity e)
    {
        Debug.Log("Entering Move State");
    }
    //Exiting The State
   public  void Run(AI_Entity e)
    {
        Debug.Log("Running Move State");
    }

    public void Exit(AI_Entity e)
    {
        Debug.Log("Exiting MoveState");
    }
}
public class InmateDrinkingState : IState
{
    //Entering The State
    public void Enter(AI_Entity e)
    {
        Debug.Log("Entering Drinking State");
    }
    //Exiting The State
    public void Run(AI_Entity e)
    {
        Debug.Log("Running Drink State");
    }

    public void Exit(AI_Entity e)
    {
        Debug.Log("Exiting Drink State");

    }
}
public class InmateEatingState : IState
{
    //Entering The State
    public void Enter(AI_Entity e)
    {
        Debug.Log("Entering Eat State");

    }
    //Exiting The State
    public void Run(AI_Entity e)
    {
        Debug.Log("Running Eating State");

    }

    public void Exit(AI_Entity e)
    {
        Debug.Log("Exiting Eating State");

    }
}

