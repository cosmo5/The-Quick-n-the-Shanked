using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AI : MonoBehaviour {
    public  enum TypeOfAI
    {
        sketchyTarget,
        targetInGang,
        normalGuard,
        towerGuard,
        GuardDog
    };
   
    public float speed;
    public float rotateSpeed;
    public float fovAngle;
    protected System.Random rnd = new System.Random()  ;
    protected bool playerInSight;
    public GameObject player;
    protected GameManager gm;
    private RaycastHit hit;
    private RaycastHit moveHit;
    public float decisionTimer;
    protected Vector3 moveDir;
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

    public bool pathFound;
    protected Vector3[] path;
    public int targetIndex;
    protected Grid grid;
    private Node myNode;
    private List<Node> neightbourNodes = new List<Node>();

    protected bool stopMovement;
    protected virtual void Start()
    {
        grid = FindObjectOfType<Grid>();
        myNode = grid.NodeFromWorldPoint(transform.position);
        myNode.movementPenalty = 50;
        foreach (Node node in neightbourNodes)
        {
            node.movementPenalty = 50   ;

        }
        neightbourNodes = grid.GetNeighbours(myNode);
        gm = FindObjectOfType<GameManager>();
        player = gm.player;
       // decisionTimer = Random.Range(5, 8);
        cachTimer = decisionTimer;
    }

    protected virtual void Think(int x, Vector3 randomDir, Quaternion startRot)
    {
    }

    protected virtual void Update()
    {
     
        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0)
        {
           
                rndNumber = rnd.Next(0,50);
                do
                {
                    randomDir = UnityEngine.Random.insideUnitSphere * 10;
                } while (Vector3.Angle(randomDir, transform.forward) < 90);

                _startRot = transform.rotation;
                decisionTimer = cachTimer;
            
           
        }

        if (pathFound)
        {
            Move();

        }
    }
    protected virtual void Think(int x, Vector3 randomDir,  Quaternion startRot, bool moving)
    {

    }
    public int DiceRoll()
    {
        int x = rnd.Next(6);
        print(x);
        return x;
    }
    public void RequestPath(Vector3 currPos, Vector3 targetPos)
    {
    
       if(!pathFound)
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
    protected virtual void Rotate(Vector3 lookDir, bool rotateBack, Quaternion startRot)
    {
        
        Quaternion rot = Quaternion.LookRotation(lookDir.normalized);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, rot, rotateSpeed * Time.deltaTime);
        
        if (rotateBack && transform.rotation == rot)
        {

          //  transform.rotation = Quaternion.RotateTowards(transform.rotation, startRot, rotateSpeed * Time.deltaTime);
        }
    }
    protected virtual void Move()
    {
        if (path!=null)
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
