using UnityEngine;
using System.Collections;
/// <summary>
/// All the different states and their functionality
/// </summary>
public class Inmate_Entity : Vehicle
{
   public GameObject movePos;
    //public float rotSpeed;
    public float moveSpeed;
    protected FiniteStateMachiene<Inmate_Entity> fsm;

    Vector3 startPos;
    void Awake()
    {
        startPos = transform.position;
        Debug.Log("RunningAwake");
        base.Start();
        fsm = new FiniteStateMachiene<Inmate_Entity>();
        fsm.Init(this, IdleState.Instance);
    }


    public FiniteStateMachiene<Inmate_Entity> FSM
    {
        get { return fsm; }
    }
  
    public override void Update()
    {
        base.Update();
        fsm.Update();
        
    }
    public void Move()
    {
       
    }

   
    //Change state func
   
  

}



enum Events
{
    Moving,
    Drinking,
    Eating
}



