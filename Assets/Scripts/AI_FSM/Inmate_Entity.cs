using UnityEngine;
using System.Collections;
/// <summary>
/// All the different states and their functionality
/// </summary>
public class Inmate_Entity:MonoBehaviour
{

    private FiniteStateMachiene<Inmate_Entity> fsm;


    void Awake()
    {
        Debug.Log("RunningAwake");
        fsm = new FiniteStateMachiene<Inmate_Entity>();
        fsm.Init(this, IdleState.Instance);
        StartCoroutine("doSomething");
    }

    //Change state func
   
  IEnumerator doSomething()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            fsm.ChangeState(MovingState.Instance);


        }
    }
}

enum Events
{
    Moving,
    Drinking,
    Eating
}



