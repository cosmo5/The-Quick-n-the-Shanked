using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Inmate : AI {
    public float dst;
  //  private List<Inmate> _Inmates = new List<Inmate>();
    
    bool playerAttack;
      

    private int rndNum;
    public Material narkMat;

    public bool willNark;
    bool moving = false;
    Guard guardToNarcTo = null;


    public bool narcing;
    Vector3 positionToMoveTo;
   
    float cachTimer;
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
        decisionTimer = Random.Range(3f, 5f);
        cachTimer = decisionTimer;
    }

    public int RndNum
    {
        set { rndNum = value; }
    }
    public Target Target
    {
        set { target = value; }
    }
    void OnDisable()
    {
        Player.onAttack -= PlayerAttack;    
    }
    protected override void Think(int x, Vector3 randomDir, Quaternion startRot)
    {
        base.Think(x, randomDir, startRot);
        
    }
    void PlayerAttack()
    {
        playerAttack = true;
        if (target != this)
        {
           
            if (Vector3.Distance(transform.position, player.transform.position) < dst)
            {
                Vector3 dir = player.transform.position - transform.position;

                Look(dir);
                if (playerInSight && willNark)
                {
                    List<GameObject> temp = new List<GameObject>();
                    foreach (Guard g in gm._guard)
                    {
                        temp.Add(g.gameObject);

                    }

                    gm.OrderList(true, true);
                    guardToNarcTo = gm._guard[0];
                    moving = true;
                    narcing = true;
                }
            }
        }
       
    }

   
   
    void FixedUpdate()
    {
        Look(player.transform.position - transform.position);
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
            rndNumber = Random.Range(0, 100); //rnd.Next(0,100);
            randomDir = Random.insideUnitSphere * 10;
            randomDir.Normalize();
            _startRot = transform.rotation;
            decisionTimer = cachTimer;
        }

        Think(rndNumber,randomDir , _startRot);
        if (positionToMoveTo != Vector3.zero)
        {
            if (transform.position == positionToMoveTo)
            {
                moving = false;
                positionToMoveTo = Vector3.zero;
            }
        }
      
        
        
	}

    
    
}
