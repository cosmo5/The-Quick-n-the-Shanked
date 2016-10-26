using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {
    protected  enum TypeOfAI
    {
        sketchyTarget,
        targetInGang,
        normalGuard,
        towerGuard,
        GuardDog
    };

    public float speed;
    public float rotateWait;
    public float rotateSpeed;
    public float fovAngle;
    protected System.Random rnd = new System.Random();
    protected bool playerInSight;
    public GameObject player;
    protected GameManager gm;
    private RaycastHit hit;
    public float decisionTimer;

    protected Vector3 randomDir;
    protected int rndNumber;
    protected TypeOfAI myType;
    protected Quaternion _startRot;
    public Target target;
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }
    protected virtual void Think(int x, Vector3 randomDir,  Quaternion startRot)
    {
       
        if (x > 30 && x < 50)
        {
            randomDir.y = 0;
            float angle = Vector3.Angle(randomDir, transform.forward);

            if ( angle < 200)
            {
                Rotate(randomDir, true, startRot);
            }
            
        }
        
    }

   
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (playerInSight)
        {
            Gizmos.DrawLine(transform.position, hit.point);

        }
    }
    protected virtual void Look(Vector3 direction)
    {
        playerInSight = false;
        float angle = Vector3.Angle(direction, transform.forward);

        if (angle < fovAngle * 0.5f)
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
    protected virtual void Rotate(Vector3 lookDir, bool rotateBack, Quaternion startRot)
    {
        
        Quaternion rot = Quaternion.LookRotation(lookDir.normalized);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotateSpeed * Time.deltaTime);

        if (rotateBack && transform.rotation == rot)
        {
            print("Running");
            transform.rotation = Quaternion.RotateTowards(transform.rotation, startRot, rotateSpeed * Time.deltaTime);
        }
    }
    protected virtual void Move(Vector3 dir)
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, dir, out hit, 30))
        {
            GetComponent<Rigidbody>().AddForce(dir * speed * Time.deltaTime, ForceMode.Impulse);
            ClampPos();
        }
    }
    void ClampPos()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, gm.xMin - 1, gm.xMax + 1);
        pos.z = Mathf.Clamp(pos.z, gm.zMin - 1, gm.Zmax + 1);

        transform.position = pos;
    }
    
}
