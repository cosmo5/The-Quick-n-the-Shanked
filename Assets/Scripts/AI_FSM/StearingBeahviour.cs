using UnityEngine;
using System.Collections.Generic;
using System.Linq;
namespace Stearing
{
    public class StearingBeahviour
    {
        const float seek = 0.9f;
        const float chase = 0.2f;
        System.Random rnd = new System.Random();
        const float evade = 0.5f;
        const float flee = 0.5f;
        const float wander = 0.6f ;
        
        public Vector3 Seek( Vehicle toAffect, Vector3 targetPosition)
        {
            Vector3 desiredVelocity = Vector3.Normalize(targetPosition - toAffect.transform.position) * toAffect.maxSpeed   ;
            return desiredVelocity;
        }

        public Vector3 Flee(Vehicle toAffect, Vector3 targetPosition)
        {
            Vector3 desiredVelocity = Vector3.Normalize(toAffect.transform.position - targetPosition) * toAffect.maxSpeed;


            return desiredVelocity;
        }

        public Vector3 Arrive( float time, Vector3 targetPos, Vehicle toAffect)
        {
            // s = d/t

             float dist = (targetPos - toAffect.transform.position).magnitude;

            float speed = dist / time;
            
            Vector3 desiredVelocity = Vector3.Normalize(targetPos - toAffect.transform.position) * speed;
            return desiredVelocity;
        }
        void OnDrawGizmos()
        {
            
        }
        public Vector3 Wander(Vehicle toAffect, float wanderRad, float wanderDst, float wanderJitter)
        {
            float theta = Random.Range(-1f, 1f) * Mathf.PI * 2;
            Vector3 wanderTarget = new Vector3(wanderRad * Mathf.Cos(theta), 0, wanderRad * Mathf.Sin(theta));
            wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f) ) * wanderJitter;//   Random.insideUnitSphere * wanderJitter; Random.insideUnitSphere * wanderJitter; 
            wanderTarget.Normalize();
            wanderTarget.y = 0;

            wanderTarget *= wanderRad;

            
            Vector3 targetLocal = wanderTarget + new Vector3(0,0, wanderDst);
            Vector3 targetWorld = toAffect.transform.TransformDirection(targetLocal);
            return (targetWorld - toAffect.transform.position).normalized;// * toAffect.maxSpeed;
        }
        public Vector3 Chase(Vehicle toAffect, Vehicle evader)
        {
            Vector3 toEvader = evader.transform.position - toAffect.transform.position;

            float relitiveHeading = Vector3.Dot(toAffect.vehicleHeading, evader.vehicleHeading);
           // Debug.Log(Vector3.Dot(toEvader, toAffect.vehicleHeading));
            if ((Vector3.Dot(toAffect.vehicleHeading, toEvader)> 0 && relitiveHeading > -0.95f )|| evader.vehicleHeading == Vector3.zero)
            {
                Debug.Log("In this");
                //Debug.Log("Return normal seek");
//                Debug.Log(evader.transform.position);
                return Seek(toAffect, evader.transform.position);
            }
            float lookAheadTime = 0;
             //lookAheadTime += TurnAroundTime(toAffect,evader.transform.position );                   //Debug.Log(lookAheadTime);
                lookAheadTime = toEvader.magnitude / (toAffect.maxSpeed + evader.maxSpeed);            //Debug.Log(lookAheadTime);

            return Seek(toAffect, (evader.vehicleHeading + evader.transform.position)  * lookAheadTime );
        }
        public Vector3 Evade(Vehicle toAffect, Vehicle pursuer)
        {
            Vector3 toPursuer = toAffect.transform.position - pursuer.transform.position;

            
            float lookAheadTime = 0;
            lookAheadTime = toPursuer.magnitude / (toAffect.maxSpeed + pursuer.maxSpeed);  
            return Flee(toAffect, pursuer.vehicleHeading * lookAheadTime );
        }
        private float TurnAroundTime(Vehicle agent, Vector3 targetPos)
        {
            Vector3 toTar = Vector3.Normalize(targetPos - agent.transform.position);
            float dot = Vector3.Dot( agent.GetComponent<Rigidbody>().velocity, toTar);

            return (dot - 1.0f) * -agent.coef;
        }

        public Vector3 ObsticleAvoidance(Vehicle agent, Vector3 centre, Vector3 hExtends)
        {
            Vector3 steeringForce = Vector3.zero;

            Vector3 desiredVel = Vector3.zero;
            GameObject intersectingObj = null;
            float dstToIP = 0;
            Vector3 objectPosLocal = Vector3.zero;

            List<Collider> cols = new List<Collider>();
            cols.AddRange( Physics.OverlapBox(centre, hExtends,agent.transform.rotation));
            cols.OrderBy(x => Vector3.Distance(x.transform.position, agent.transform.position));
            if (cols[0] == agent.GetComponent<Collider>())
            {
                cols.RemoveAt(0);
            }
            if (cols.Count > 0 )
            {
                Debug.Log("Avoiding" + cols[0].name);
                float multi =1.0f + (agent.GetComponent<Collider>().bounds.extents.z - cols[0].transform.position.x) / agent.GetComponent<Collider>().bounds.extents.z;
                Vector3 ColLocal = agent.transform.InverseTransformPoint(cols[0].transform.position);
                steeringForce.z = (cols[0].bounds.extents.z - ColLocal.z) * multi;

                float breakWeight = 0.2f;

                steeringForce.x = (cols[0].bounds.extents.x - ColLocal.x) * breakWeight;


            }
            return steeringForce;
        }
        public Vector3 Calc( Vehicle toAffect, Vehicle target, List<string> moves, Vector3 wanderTarg)
        {
            bool seekTrue = false;
            bool arriceTrue = false;
            bool chaseTrue = false;
            bool evadeTrue = false;
            bool wanderTrue = false;
            bool fleeTrue = false;
            bool obsticle = false;

            Vector3 _steeringForce =Vector3.zero;
            Vector3 force = Vector3.zero;
            foreach (string s in moves)
            {
                if (s == "Seek")
                {
                    seekTrue = true;
                }
                if (s == "Flee")
                {
                    fleeTrue = true;
                }
                if (s == "Arrive")
                {
                    arriceTrue = true;
                }
                if (s == "Chase")
                {
                    chaseTrue = true;
                }
                if (s == "Evade")
                {
                    evadeTrue = true;
                }
                if (s == "Wander")
                {
                    wanderTrue = true;
                }
                if (s == "OBST")
                {
                    obsticle = true;
                }
            }
          
            if (seekTrue)
            {
                force = Seek( toAffect, target.transform.position) * 1;
                if (!AccumulateForce(toAffect, _steeringForce, force))
                {
                    _steeringForce = force;
                    return force;
                }
            }
            if (fleeTrue)
            {
                force = Flee(toAffect, target.transform.position) * 1;

                if (!AccumulateForce(toAffect, _steeringForce, force))
                {
                    _steeringForce = force;

                    return force;
                }
            }
            if (arriceTrue  )
            {
                force = Arrive(toAffect.stoppingTime, target.transform.position, toAffect) * 1f / 0.1f;
                if (!AccumulateForce(toAffect, _steeringForce, force))
                {
                    _steeringForce = force;

                    return force;
                }
            }
            if (chaseTrue&& Random.Range(0.0f, 10) > chase)
            {
                force = Chase( toAffect, target) * 0.5f / chase;
                if (!AccumulateForce(toAffect, _steeringForce, force))
                {
                    _steeringForce = force;

                    return force;
                }
            }

            if (evadeTrue && Random.Range(0.0f, 10) > evade)
            {
                force = Evade(toAffect, target) * 0.5f / chase;
                if (!AccumulateForce(toAffect, _steeringForce, force))
                {
                    _steeringForce = force;

                    return force;
                }
            }
            if (obsticle && Random.Range(0.0f, 10) > wander)
            {
                force = ObsticleAvoidance(toAffect, toAffect.centre, toAffect.hExtends) * 15 / wander;
                if (!AccumulateForce(toAffect, _steeringForce, force))
                {
                    _steeringForce = force;
                    return force;
                }
            }
            if (wanderTrue && Random.Range(0.0f, 3) > wander)
            {
                force = Wander(toAffect, toAffect.wanderRad , toAffect.wanderDst, toAffect.wanderJitter) * 2  / wander;
                if (!AccumulateForce(toAffect, _steeringForce, force))
                {
                    _steeringForce = force;
                    
                    return force;
                }
            }

            return force;
        }

       

        public bool AccumulateForce(Vehicle toAffect, Vector3 force, Vector3 forceToAdd)
        {
        
            float mag = force.magnitude;

            float magRemaining = toAffect.maxForce - mag;

            if (magRemaining <= 0)
                return false;

            float magToAdd = forceToAdd.magnitude;
    
           // Debug.Log(magRemaining);
            if (magToAdd < magRemaining)
            {
                force += forceToAdd;
            }
            else
            {
                force += Vector3.Normalize(forceToAdd) * magRemaining;
            }
            return true;
        }

    }


}
