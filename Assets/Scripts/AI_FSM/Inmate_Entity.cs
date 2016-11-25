using UnityEngine;
using System.Collections;
/// <summary>
/// All the different states and their functionality
/// </summary>
public class Inmate_Entity : MovingEntity
{
   public GameObject movePos;
    public float rotSpeed;
    public float moveSpeed;
    protected FiniteStateMachiene<Inmate_Entity> fsm;

    Vector3 startPos;
    void Awake()
    {
        startPos = transform.position;
        Debug.Log("RunningAwake");

        fsm = new FiniteStateMachiene<Inmate_Entity>();
        fsm.Init(this, IdleState.Instance);
    }


    public FiniteStateMachiene<Inmate_Entity> FSM
    {
        get { return fsm; }
    }
  
    void Update()
    {
        fsm.Update();
    }
    public void Move()
    {
        Vector3 dir = movePos.transform.position - transform.position;
        dir.Normalize();
        Debug.Log("Moving");
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);

        transform.position = Vector3.MoveTowards(transform.position, movePos.transform.position, moveSpeed * Time.deltaTime);
    }

   
    //Change state func
   
  

}



enum Events
{
    Moving,
    Drinking,
    Eating
}



