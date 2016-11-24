using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Target : AI {
    [Range(0,100)]  
    public float laziness;
    [Range(0, 100)]
    public float happiness;
    [Range(0, 100)]
    public float scared;

    private float originalScared;
    public float scaredMultiplyer;

    private bool gang;
    private GameObject gangPos;
    public bool stayLockedToPlayer;
    public List<Guard> guards = new List<Guard>();
    public GameObject something;
    private bool targetAtStart;
    public bool targetAtPos;
    private Vector3 obsticleMovePos;
    private bool moveBackToStart;
    float startdst = 0;
    private Vector3 prevPos;
    private bool doOnce;
    private bool tooScry;

    Guard guardToRunTo;
    // Use this for initialization
     protected override void Start() {
        //Call Base AI Start Function
        base.Start();
        startPos = transform.position;
        //Populate the guard List
        guards = gm._guard;

        targetAtPos = false;
        targetAtStart = true;
        cachTimer = decisionTimer;

        
        if (myType == TypeOfAI.sketchyTarget)
            scared = 30;

        originalScared = scared;
    }
    void FixedUpdate()
    {
       // Look(moveDir, moving);
        if (moving )
        {
            RequestPath(transform.position, movePos, false);
        }
    }

    private void RunToGuard()
    {
        gm.OrderList(true, transform.position);
        movePos = gm._guard[0].transform.position;
        guardToRunTo = gm._guard[0];
        tooScry = true;
        stopMovement = false;
        moving = true;
        targetAtPos = false;
    }
    // Update is called once per frame
    protected override void Update () {
        base.Update();
        if (scared >= 99 && tooScry == false)
        {
            //run to guard
            if (pathFound)
            {
                ClearPath();
            }
            RunToGuard();
        }

        if(!gm.yardOver)
            Think(rndNumber, randomDir, _startRot, moving);
        
        

        if (Vector3.Distance(transform.position, movePos) < 1 && !targetAtPos )
        {
            //Do Something
            targetAtPos = true;
            moving = false;
            if (tooScry)
            {
               guardToRunTo.SendMessage("TargetScared");
            }

        }
        if (Vector3.Distance(transform.position, startPos) < 0.5f && moveBackToStart)
        {
            //Do Something
            targetAtPos = false;
            targetAtStart = true;
            moving = false;
        }

        

     
    }
    private void LockOnToPlayer()
    {
        Debug.Log("Player Near Target, Target Sketchy");
        Rotate(player.transform.position - transform.position, false, transform.rotation);
        if(moving)
          scared +=( (startdst - Vector2.Distance(transform.position, player.transform.position) ) * (scaredMultiplyer / 3));
        else
            scared += ((startdst - Vector2.Distance(transform.position, player.transform.position)) * (scaredMultiplyer ));

        scared = Mathf.Clamp(scared, 0, 100);
        speed -= ((startdst - Vector2.Distance(transform.position, player.transform.position)));
        speed = Mathf.Clamp(speed, 40, 75);
        //stopMovement = true;
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
        
        movePos = startPos;
        moveDir = startPos - transform.position;
        moveBackToStart = true;
        targetAtPos = false;
        moving = true;
    }
    private void SelectGuard()
    {
        int x = Random.Range(0, guards.Count);
        Vector3 guardpos = guards[x].transform.position;
        movePos = guardpos;
        moving = true;
      
    }

    protected override void Think (int x, Vector3 randomDir, Quaternion startRot, bool moving)
    {

        base.Think(x, randomDir, startRot, moving);
        switch (myType)

        {
            case TypeOfAI.sketchyTarget:
                //Everything the target can do when stationary, 
                //this includes finding places to walk to, randomly rotating, sketching out whenn player is close
                if (Vector3.Distance(transform.position, player.transform.position) < 1.5f || stayLockedToPlayer)
                {
                    if (doOnce)
                    {
                        startdst = Vector2.Distance(transform.position, player.transform.position);

                        doOnce = false;
                    }   
                    //Freak Out
                    if(!tooScry)
                         LockOnToPlayer();
                    
                }
                if (Vector3.Distance(transform.position, player.transform.position) > 2.5f && !tooScry)
                {
                    scared = Mathf.Lerp(scared, originalScared, Time.deltaTime);
                    stayLockedToPlayer = false;
                    stopMovement = false;
                    doOnce = true;
                    startdst = 0;
                }

                #region moving
                if (!moving)
                {
                   
                    if (!stayLockedToPlayer)
                    {
                        if (x > 10 && x < 20)
                        {
                            randomDir.y = 0;
                            float angle = Vector3.Angle(randomDir, transform.forward);

                            if (angle < 200)
                            {
                                //Dice roll Greater than 4 rotate back
                                Rotate(randomDir, DiceRoll(UnityEngine.Random.Range(0, 6)), startRot);
                            }

                        }

                        if (x > 20 && x < 30)
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

                    }



                }
                #endregion
                if (moving)
                {

                }


                break;
            case TypeOfAI.targetInGang:
                break;

            default:
                break;
        }
        
    }
}
