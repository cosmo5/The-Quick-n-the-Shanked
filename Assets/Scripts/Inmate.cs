using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Inmate : MonoBehaviour {
    GameObject player;
    public float dst;
    GameManager gm;
    private GameObject target;
    private List<Inmate> _Inmates = new List<Inmate>();
    public float speed = 10;
    bool playerAttack;
    bool decision;
    float decisionTimer = 3;

    private int rndNum;
    public Material narkMat;

    public bool willNark;
    bool moving = false;
    Guard guardToNarcTo = null;

    System.Random rnd;

    Vector3 positionToMoveTo;
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        rnd = new System.Random();
        Player.onAttack += PlayerAttack;
        gm = FindObjectOfType<GameManager>();
        if (rndNum > 35)
        {
            willNark = true;
            GetComponent<Renderer>().material = narkMat;
        }
    }

    public int RndNum
    {
        set { rndNum = value; }
    }
    public GameObject Target
    {
        set { target = value; }
    }
    void OnDisable()
    {
        Player.onAttack -= PlayerAttack;
    }
	void PlayerAttack()
    {
        playerAttack = true;
        if (target != this)
        {
           
            if (Vector3.Distance(transform.position, player.transform.position) < dst)
            {
                Vector3 dir = player.transform.position - transform.position;

                RaycastHit hit;
                if (Physics.Raycast(transform.position, dir.normalized, out hit, 15))
                {
                    if (hit.transform.tag == "Player")
                    {
                        if (willNark)
                        {
                            Debug.Log("Your in my sight");

                            decision = true;
                        }
                       
                    }
                }
            }
        }
       
    }

    void Decisions(bool attack)
    {
        int x = Random.Range(0, 100);

        if (attack)
        {
            if (willNark)
            {
                List<GameObject> temp = new List<GameObject>();
                foreach (Guard g in gm._guard)
                {
                    temp.Add(g.gameObject);

                }

                gm.OrderList(true, true);
                guardToNarcTo = gm._guard[0];
                moving = true;
            }
        }
        else
        {
            if (x >0 && x < 70)
            {
                //Do noThing??

         
            }

            if (x > 80 && x <90)
            {

                Vector3 rndPos = new Vector3((float)rnd.NextDouble() * (gm.xMax - gm.xMin) + gm.xMin,
                 1, (float)rnd.NextDouble() * (gm.Zmax - gm.zMin) + gm.zMin);
                ;


                while (gm.CheckDistanceOthers(rndPos, false))
                {
                    rndPos = new Vector3((float)rnd.NextDouble() * (gm.xMax - gm.xMin) + gm.xMin,
             1, (float)rnd.NextDouble() * (gm.Zmax - gm.zMin) + gm.zMin);

                }



                moving = true;
               
                positionToMoveTo = rndPos;
            }

        }
        
    }
    void Move(Vector3 dir)
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, dir, out hit, 30))
        {
            GetComponent<Rigidbody>().AddForce(dir * speed * Time.deltaTime, ForceMode.Impulse);
            ClampPos();
        }
    }
    void FixedUpdate()
    {
        if (decision)
        {
            Decisions(playerAttack);
            decision = false;  
        }

        if (moving && !playerAttack)
        {
            
            Move((positionToMoveTo - transform.position).normalized);
        }
        if (guardToNarcTo != null)
        {
            Vector3 dir = guardToNarcTo.transform.position - transform.position;
            dir.Normalize();
            Move(dir);
        }
    }
    // Update is called once per frame
    void Update () {
        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0)
        {
            decision = true;
            decisionTimer = 3;
        }

        if (positionToMoveTo != Vector3.zero)
        {
            if (transform.position == positionToMoveTo)
            {
                moving = false;
                positionToMoveTo = Vector3.zero;
            }
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
