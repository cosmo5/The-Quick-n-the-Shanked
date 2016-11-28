using UnityEngine;
using System.Collections.Generic;
using Stearing;
using System.Collections;
/// <summary>
/// 
/// </summary>
public class Vehicle : MovingEntity {
   public StearingBeahviour  sB;
   protected Vector3 desiredVel = Vector3.zero;
    protected Rigidbody rigi;
    public GameObject target;
    public float stoppingTime;
    public List<string> nextMoves  = new List<string>();
    public Vehicle targetVehicle;
    public GameObject wanderTarget;
    public float coef;
    public GameObject Vpos;
    public Vector3 centre;
    public Vector3 hExtends;
    public float wanderJitter;
    public float wanderRad;
    public float wanderDst;
    public float minBoxLength;
    public bool test;
    public virtual void Start()
    {
         desiredVel = Vector3.zero;

        sB = new StearingBeahviour();
        rigi = GetComponent<Rigidbody>();
        
    }

            
    
    public virtual void Update()
    {
        desiredVel = Vector3.zero;

        nextMoves.Clear();
        if (transform.tag == "Guard")
        {
//            nextMoves.Add("Chase");

        }
        else
        {
            nextMoves.Add("Wander");

            if (Vector3.Distance(transform.position, targetVehicle.transform.position) < 2)
            {

                     // nextMoves.Add("Evade");
            }
            else
            {

              //      nextMoves.Add("Arrive");

            }

            if (Vector3.Distance(transform.position, wanderTarget.transform.position) < 1)
            {

                //nextMoves.Add("Arrive");
            }
        }
        hExtends.x = GetComponent<BoxCollider>().size.x;
        hExtends.z = minBoxLength + (rigi.velocity.magnitude / maxSpeed);
        centre = transform.position + ( transform.forward* ( hExtends.z/2));
        

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
       // Gizmos.DrawWireCube(centre, hExtends);
    }
    private void UpdateFacingDir(Vector3 heading)
    {
        Quaternion rot = Quaternion.identity;
         if (heading != Vector3.zero)
             rot = Quaternion.LookRotation( heading, Vector3.up);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotSpeed * Time.deltaTime);

        
    }
    public void ClampVel()

    {
        if (rigi.velocity.magnitude > maxSpeed)
        {
            rigi.velocity = rigi.velocity.normalized * maxSpeed;
        }
    }
    public void MoveNew()
    {
        //resetVel
        rigi.velocity = Vector3.zero;

        //Get Combined Force From stearingBehaviours

        desiredVel = sB.Calc(this, targetVehicle, nextMoves, wanderTarget.transform.position);
        desiredVel.y = 0;
//        Vpos.transform.position = desiredVel;
        //calculate acceleration 
        Vector3 acc = desiredVel / mass;

        //Update Velocity

        velocity += acc;
        rigi.velocity = velocity;

        //Clamp Velocity
        ClampVel();

        //Update Rot

        vehicleHeading = rigi.velocity.normalized;
        UpdateFacingDir(vehicleHeading);






        Debug.DrawLine(transform.position,   vehicleHeading *2 +transform.position , Color.red);


    }
}
