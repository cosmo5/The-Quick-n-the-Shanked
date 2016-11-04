using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Target : AI {
    private bool gang;
    private GameObject gangPos;
    public bool stayLockedToPlayer;
    public GameObject[] keyAreas;
    public List<Guard> guards = new List<Guard>();
    public GameObject something;
    private bool targetAtStart;
    public bool targetAtPos;
    private Vector3 obsticleMovePos;
    private bool moveBackToStart;
    // Use this for initialization
     protected override void Start() {
        base.Start();
        startPos = transform.position;
        keyAreas = GameObject.FindGameObjectsWithTag("KeyMap");
        targetAtPos = false;
        targetAtStart = true;
        cachTimer = decisionTimer;
       // guards.AddRange(gm._guard);
    }
    void FixedUpdate()
    {
        Look(moveDir, moving);
        if (moving)
        {
            Rotate(moveDir, false, transform.rotation);
            
            if (!ObsticleInWay)
            {
                moveDir = movePos - transform.position;
                Move(moveDir);
            }   
            else
            {
                moveDir = Quaternion.AngleAxis(30 , Vector3.up) * moveDir;

            }
           
         
        }
     
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
        if (!moving)
        {
            Think(rndNumber, randomDir, _startRot);

        }
        else if (moving)
        {
            if (Vector3.Distance(transform.position, movePos) < 0.2f && !targetAtPos )
            {
                //Do Something
                targetAtPos = true;
                moving = false;
            }
            if (Vector3.Distance(transform.position, startPos) < 0.2 && moveBackToStart)
            {
                //Do Something
                targetAtPos = false;
                targetAtStart = true;
                moving = false;
            }

        }

     
    }
    private void LockOnToPlayer()
    {
        Debug.Log("Player Near Target, Target Sketchy");
        Rotate(player.transform.position - transform.position, false, transform.rotation);
        stayLockedToPlayer = true;
    }

    private void KeyAreas(int x )
    {
        
        Vector3 posToWalkTo = Vector3.zero;
        switch (x)
        {
            case 36:
                //'Showers'
                posToWalkTo = keyAreas[0].transform.position;
                something = keyAreas[0];
                break;
            case 37:
                posToWalkTo = keyAreas[1].transform.position;
                something = keyAreas[1];
                break;
            case 38:
                posToWalkTo = keyAreas[2].transform.position;
                something = keyAreas[2];
                break;
            case 39:
                posToWalkTo = keyAreas[3].transform.position;
                something = keyAreas[3];
                break;
            default:
                break;
        }
        moveDir = posToWalkTo - transform.position;
        movePos = posToWalkTo;
        moving = true;
        targetAtStart = false;
    }
    private void WalkToStart()
    {
        moving = true;
        movePos = startPos;
        moveDir = startPos - transform.position;
        moveBackToStart = true;
        targetAtPos = false; 
    }
    private void SelectGuard()
    {

    }

    protected override void Think(int x, Vector3 randomDir, Quaternion startRot)
    {
        base.Think(x, randomDir, startRot);
       
        switch (myType)

        {
            case TypeOfAI.sketchyTarget:

                if (Vector3.Distance(transform.position, player.transform.position) < 2)
                {
                    //Freak Out
                    LockOnToPlayer();
                }
                if (Vector3.Distance(transform.position, player.transform.position) > 5)
                {
                    stayLockedToPlayer = false;
                }


                if (x > 10 && x < 20)
                {
                    randomDir.y = 0;
                    float angle = Vector3.Angle(randomDir, transform.forward);

                    if (angle < 200)
                    {
                        Rotate(randomDir, true, startRot);
                    }

                }

                if ( x > 20 && x < 30)
                {
                    if (targetAtPos)
                    {
                        //WalkToStartPoint
                        WalkToStart();
                    }
                    
                }

                if (x > 30 && x < 35)
                {
                    //Walk To Guard
                    Debug.Log("Target Walking to Guard");
                    SelectGuard();
                }
                if (x > 35 && x < 40)
                {
                    //Walk To Key Area
                    Debug.Log("Target Walking to key area");
                    KeyAreas(x);
                }



                break;
            case TypeOfAI.targetInGang:
                break;

            default:
                break;
        }
        
    }
}
