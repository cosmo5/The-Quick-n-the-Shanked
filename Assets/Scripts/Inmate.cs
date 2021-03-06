﻿using UnityEngine;
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
    public Guard guardToNarcTo = null;


    public bool narcing;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
        rnd = new System.Random();
        Player.onAttack += PlayerAttack;
        
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
    public Target Target
    {
        set { target = value; }
    }
    void OnDisable()
    {
        Player.onAttack -= PlayerAttack;    
    }
    protected override void Think(int x, Vector3 randomDir, Quaternion startRot, bool moving)
    {
        base.Think(x, randomDir, startRot, moving);
        

    }
    void PlayerAttack()
    {
        playerAttack = true;
       
        if (Vector3.Distance(transform.position, player.transform.position) < dst)
        {
            Vector3 dir = player.transform.position - transform.position;

            Look(dir, moving);
            if (playerInSight && willNark)
            {
                List<GameObject> temp = new List<GameObject>();
                foreach (Guard g in gm._guard)
                {
                    temp.Add(g.gameObject);

                }

                gm.OrderList(true, transform.position);
                guardToNarcTo = gm._guard[0];
                moving = true;
                narcing = true;
            }
            
        }
       
    }

   
   
    void FixedUpdate()
    {
        if(!moving)
         Look(player.transform.position - transform.position, moving);
        if (moving && !playerAttack )
        {
            
            RequestPath(transform.position, movePos, false);
        }
        if (guardToNarcTo != null)
        {
            RequestPath(transform.position, guardToNarcTo.transform.position, true);
        }
    }
    // Update is called once per frame
   protected override void Update () {

        base.Update();
  
            Think(rndNumber, randomDir, _startRot, moving);

        
      
      
        
        
	}

    
    
}
