using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AI : MonoBehaviour
{
    public enum TypeOfAI
    {
        sketchyTarget,
        targetInGang,
        normalGuard,
        towerGuard,
        GuardDog
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

    public bool pathFound;
    protected Vector3[] path;
    public int targetIndex;
    protected Grid grid;
    private Node myNode;
    private List<Node> neightbourNodes = new List<Node>();
    protected bool stopMovement;
    private bool imMadGonKill;
    private bool gettingWater;
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
        // Move to Respective Areas
        print("BASE AI");

        movePos = GameObject.Find("Cells").transform.position;
        moving = true;
    }
    protected void ClearPath()
    {
        path = null;
        pathFound = false;
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
                waterMeter += 5;
                if (waterMeter >= 95)
                {
                    gettingWater = false;
                }
            }
        }

        if (pathFound)
        {
            Move();

        }
    }

    protected virtual void Think(int x, Vector3 randomDir, Quaternion startRot, bool _moving)
    {


        if (waterMeter <= 30)
        {
            //Get Water
            if (DiceRoll())
            {
                Debug.Log("Time To get Water");
                movePos = GameObject.Find("Water").transform.position;
                moving = true;
                gettingWater = true;
            }

            if (DiceRoll() && foodMeter <= 50)
            {
                //Kill Someone
                Debug.Log("Moving To kill Player");
                Inmate toKill = gm._inmates[UnityEngine.Random.Range(0, gm._inmates.Count)];
                movePos = toKill.transform.position;
                moving = true;
                imMadGonKill = true;
            }

        }


    }

    public bool DiceRoll()
    {
        int randNum = UnityEngine.Random.Range(0, 6);
        int x = rnd.Next(6);
        if (x == randNum)
        {
            return true;
        }
        return false;
    }
    public void RequestPath(Vector3 currPos, Vector3 targetPos)
    {

        if (!pathFound)
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
