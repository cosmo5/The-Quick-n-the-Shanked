using UnityEngine;
using System.Collections;

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
    protected virtual void Start()
    {
        
        gm = FindObjectOfType<GameManager>();
        player = gm.player;
       // decisionTimer = Random.Range(5, 8);
        cachTimer = decisionTimer;
    }

  
    protected virtual void Update()
    {
        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0)
        {
            rndNumber = Random.Range(0, 40); //rnd.Next(0,100);

            do
            {
                randomDir = Random.insideUnitSphere * 10;
            } while (Vector3.Angle(randomDir, transform.forward) < 90);

            _startRot = transform.rotation;
            decisionTimer = cachTimer;
        }
    }
    protected virtual void Think(int x, Vector3 randomDir,  Quaternion startRot)
    {

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
            ObsticleInWay = false;
           
            for (int i = 0; i < 3; i++)
            {
                RaycastHit hit;
                Vector3 pos = ((transform.GetChild(0).transform.position - transform.right * 0.2f) + transform.right * i * 0.2f);
                Vector3 dir = movePos - pos; 
                if (Physics.Raycast(pos, dir.normalized, out hit, 5))
                {
                    if (hit.transform.tag == "Inmate" || hit.transform.tag == "Guard")
                    {
                        float dstToTarget = Vector3.Distance(transform.position, movePos);
                        if (hit.distance < dstToTarget)
                        {
                            if (hit.distance < 1)
                            {
                                ObsticleInWay = true;

                            }

                        }

                    }

                }
                Debug.DrawRay((transform.GetChild(0).transform.position - transform.right * 0.2f) + transform.right * i * 0.2f, dir.normalized * 15, Color.blue);
            }

        }
    }
    void OnDrawGizmos()
    {
     


    }
    protected virtual void Rotate(Vector3 lookDir, bool rotateBack, Quaternion startRot)
    {
        
        Quaternion rot = Quaternion.LookRotation(lookDir.normalized);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);
        
        if (rotateBack && transform.rotation == rot)
        {

          //  transform.rotation = Quaternion.RotateTowards(transform.rotation, startRot, rotateSpeed * Time.deltaTime);
        }
    }
    protected virtual void Move(Vector3 dir)
    {
      
        GetComponent<Rigidbody>().velocity = dir.normalized * speed * Time.deltaTime;
        ClampPos();

    }
    void ClampPos()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, gm.xMin - 1, gm.xMax + 1);
        pos.z = Mathf.Clamp(transform.position.z, gm.zMin - 1, gm.Zmax + 1);

        transform.position = pos;
    }
    
}
