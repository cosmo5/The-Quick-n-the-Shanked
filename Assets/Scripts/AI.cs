using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
public class AI : MonoBehaviour
{
    public enum TypeOfAI
    {
        sketchyTarget,
        targetInGang,
        normalGuard,
        towerGuard,
        GuardDog,
        Inmate
    };
    [Range(0, 100)]
    public float waterMeter;
    [Range(0, 100)]
    public float foodMeter;
    [Range(0, 100)]
    public float happy;
    [Range(0, 100)]
    public float energy;

    protected float baseLose = 5;
    public float speed;
    public float rotateSpeed;
    public float fovAngle;
    protected System.Random rnd = new System.Random();
    protected bool playerInSight;
    public GameObject player;
    protected GameManager gm;
    private RaycastHit hit;
    private RaycastHit moveHit;
    public float decisionTimer;
    protected Vector3 moveDir;
    [SerializeField]
    protected Vector3 movePos;
    protected Vector3 startPos;
    protected Vector3 randomDir;
    protected int rndNumber;
    public TypeOfAI myType;
    protected Quaternion _startRot;
    public Target target;
    protected float cachTimer;
    public bool ObsticleInWay;
    public bool moving;
    public Vector3 offset;
    public GameObject[] keyAreas;

    public Water myTap;
    public bool pathFound;
    protected Vector3[] path;
    public int targetIndex;
    protected Grid grid;
    private Node myNode;
    private List<Node> neightbourNodes = new List<Node>();
    protected bool stopMovement;
    private bool imMadGonKill;
    public bool gettingWater;
    bool doOnetime;
    Water water;
    private bool waterRequested = false;
    protected bool waiting;
    protected virtual void Start()
    {
        waterMeter = UnityEngine.Random.Range(50, 100);
        foodMeter = UnityEngine.Random.Range(50, 100);
        happy = UnityEngine.Random.Range(50, 100);
        energy = UnityEngine.Random.Range(50, 100);
        grid = FindObjectOfType<Grid>();
        myNode = grid.NodeFromWorldPoint(transform.position);
        myNode.movementPenalty = 50;
        foreach (Node node in neightbourNodes)
        {
            node.movementPenalty = 50;

        }
        keyAreas = GameObject.FindGameObjectsWithTag("KeyMap");
        GameManager.newHour += NewHour;
        GameManager.yardTimeOver += OnYardTimeOver;
        neightbourNodes = grid.GetNeighbours(myNode);
        gm = FindObjectOfType<GameManager>();
        player = gm.player;
        // decisionTimer = Random.Range(5, 8);
        cachTimer = decisionTimer;
        water = FindObjectOfType<Water>();
        startPos = transform.position;
    }
    public bool GettingWater
    {
        get { return gettingWater; }
    }
    void OnDisable()
    {
        GameManager.yardTimeOver -= OnYardTimeOver;
    }


    private void NewHour()
    {
        waterMeter -= baseLose * gm.dayMultiplyer;
        foodMeter -= baseLose * gm.dayMultiplyer;
        if (foodMeter < 50 || waterMeter < 50)
        {
            happy -= baseLose * gm.dayMultiplyer;
        }

        if (foodMeter > 80)
            energy += baseLose * gm.dayMultiplyer;
        else
            energy -= baseLose * gm.dayMultiplyer;
    }
    protected virtual void OnYardTimeOver()
    {
        // Move to cell Area
        print("BASE AI");

        movePos = GameObject.Find("Cells").transform.position;
        moving = true;
    }
    protected void ClearPath()
    {
        path = null;
        pathFound = false;
    }

    public void GainWater(float amount)
    {
        waterMeter += amount;
        waterMeter = Mathf.Clamp(waterMeter, 0, 100);
    }
    protected virtual void Update()
    {


        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0)
        {
            rndNumber = rnd.Next(0, 50);
            do
            {
                randomDir = UnityEngine.Random.insideUnitSphere * 10;
            } while (Vector3.Angle(randomDir, transform.forward) < 90);

            _startRot = transform.rotation;
            decisionTimer = cachTimer;
        }

        if (Vector3.Distance(transform.position, movePos) < 1)
        {
            if (gm.yardOver)
            {
                gameObject.SetActive(false);

            }
            if (imMadGonKill)
            {
                //Stab

            }
            if (gettingWater)
            {
                

                if (!waterRequested)
                {
                    myTap.RequestWater(this);
                    waterRequested = true;
                }

                
                if (myTap.beingUsed)
                {
                  

                    Vector3 dir =  myTap.transform.position - new Vector3(myTap.transform.position.x, myTap.transform.parent.transform.position.y, myTap.transform.parent.transform.position.z);
                    waiting = true;

                    movePos = myTap.transform.position + ( myTap._ai.IndexOf(this) * myTap.transform.forward * 0.5f);   //myTap.transform.position + (myTap.queueCount * myTap.transform.forward);
                    if (myTap.curr1 == this)
                    {
                        movePos = myTap.transform.position;

                    }
                    else
                    {
                        stopMovement = false;
                    }
                        
                    if (DiceRoll(UnityEngine.Random.Range(0, 50)) && happy <= 30)
                    {
                        print("im shankin you");
                    }
                   

                }
                if (waterMeter >= 95f)
                {
                    Debug.Log("Running");
                    movePos = startPos;
                    moving = true;
                    gettingWater = false;
                    myTap = null;
                    waiting = false;
                }
            }
        }

        if (pathFound)
        {
            Move();

        }
    }
    private void ChooseTap()
    {
        gm.waterTaps.Sort(delegate (Water a, Water b) {
            return (a.GetComponent<Water>().queueCount).CompareTo(b.GetComponent<Water>().queueCount);
        });

        myTap = gm.waterTaps[0];
      

    }
    protected virtual void Think(int x, Vector3 randomDir, Quaternion startRot, bool _moving)
    {


        if (waterMeter <= 30)
        {
            //Get Water
            if (DiceRoll(UnityEngine.Random.Range(0,6)))
            {
                if (myTap == null)
                     ChooseTap();

                if(!waiting)
                  movePos = myTap.transform.position;

                moving = true;
                gettingWater = true;
                print("GettingWater");
            }
            

            if (DiceRoll(UnityEngine.Random.Range(0, 15)) && foodMeter <= 30 && happy <= 30)
            {
                //Kill Someone
 
                Inmate toKill = gm._inmates[UnityEngine.Random.Range(0, gm._inmates.Count)];
                movePos = toKill.transform.position;
                moving = true;
                imMadGonKill = true;
            }

        }


    }

    public bool DiceRoll(int rndNum)
    {
       
        int x = rnd.Next(6);
        if (x == rndNum)
            return true;
        else
            return false;
    }
    public void RequestPath(Vector3 currPos, Vector3 targetPos, bool canOverride)
    {

        if (!pathFound || canOverride)
        {
            PathRequestManager.RequestPath(currPos, targetPos, OnPathFound);
            targetIndex = 0;
        }

    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            doOnetime = true;
            pathFound = true;
        }
    }
    protected virtual void Look(Vector3 direction, bool moving)
    {
        playerInSight = false;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle < fovAngle * 0.5f && !moving)
        {
            if (Physics.Raycast(transform.GetChild(0).position, direction.normalized, out hit, 30))
            {
                if (hit.transform.tag == "Player")
                {
                    playerInSight = true;
                }
                else
                {
                    playerInSight = false;
                }
            }
        }
        if (moving)
        {
            RaycastHit hitInfo;
            for (int i = 0; i < 3; i++)
            {
                if (Physics.Raycast((transform.position - transform.right * 0.1f) + transform.right * (i * 0.15f), direction, out hitInfo, 0.5f, gm.inmateMask) && doOnetime)
                {
                    RequestPath(transform.position, movePos, true);
                    doOnetime = false;
                    break;
                }
                Debug.DrawRay((transform.position - transform.right * 0.1f) + transform.right *    ( i * 0.15f ), direction, Color.red);
            }
        }
       
    }
    void OnDrawGizmos()
    {
        if (gm.drawGizmosPath)
        {
            if (path != null && pathFound)
            {
                for (int i = targetIndex; i < path.Length; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(path[i], Vector3.one * 0.5f);

                    if (i == targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }
        }

    }
    protected virtual void Rotate(Vector3 lookDir, bool rotateBack, Quaternion startRot)
    {
        Quaternion rot = Quaternion.LookRotation(lookDir.normalized);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, rot, rotateSpeed * Time.deltaTime);

        if (rotateBack && transform.rotation == rot)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, startRot, rotateSpeed * Time.deltaTime);
        }
    }
    protected virtual void Move()
    {
        if (path != null)
        {
            Vector3 currentWaypoint = Vector3.zero;
            if (targetIndex >= path.Length)
            {
                moving = false;
                pathFound = false;
            }
            else
            {
                currentWaypoint = path[targetIndex];
                if (Vector3.Distance(transform.position, currentWaypoint) < 0.2f)
                {
                    targetIndex++;
                }
            }
            if (!stopMovement)
            {
                Vector3 dir = currentWaypoint - transform.position;
                
                Look(dir, moving);
               
                GetComponent<Rigidbody>().velocity = dir.normalized * speed * Time.deltaTime;
                Rotate(dir, false, transform.rotation);
                ClampPos();
                
              
            }
        }
    }
    void ClampPos()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, gm.xMin - 1, gm.xMax + 1);
        pos.z = Mathf.Clamp(transform.position.z, gm.zMin - 1, gm.Zmax + 1);

        transform.position = pos;
    }

}
