using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
public class GameManager : MonoBehaviour {
    public GameObject player;                   /// 
    public GameObject[] guardSpots;             ///
    public GameObject targetObj;                ///
    public Guard guard;                         ///References to objects for instantiating 
    public Inmate inmate;                       ///
    public Target target;                      ///

    //Lists to hold instantiated objects
    public List<Inmate> _inmates = new List<Inmate>();      
    public List<Guard> _guard = new List<Guard>();
    public int guardCount;

    public int numInmates;                      ///Number of inmates in the yard
  
    public Material targetMat;                  // Material for target
  
    public float xMin, xMax, zMin, Zmax;        //Bounds of the screen
   
    
    public float dstFromGuards;                 //Min distance from guards the inmates can spawn
    public float gameTimer = 120f;              //How long you have till the game ends
    public float dstFromInmates;                //min Distace from other inmates
    public bool playerCaught;                   //Has the player been caught?
    public bool endGame;                        //Is the game Over

    System.Random rnd;                          //System random variable used for deciding if inmate is a 'Narc' and for target inmate
    int numPPLGroup = 6;    

    
    int i = 0;

    public Text text;                       //  Text objects to show the
    public Text timerTxt;                   //  timer and game over text

    public GameObject[] mapKeyPoints;

    public GameObject spawnObj;
    public GameObject[] spawnPositions;
    public int spawnCount;
    public int numInmatesPerGroup;
    private int cachInmateNum;
    //gets the target inmate
    public Target Target()
    {
        return target;  
    } 
        

    // Use this for initialization
    void Awake() {
        
        player = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        text = player.GetComponentInChildren<Text>();
        text.gameObject.SetActive(false);
        rnd = new System.Random();
        cachInmateNum = numInmatesPerGroup;
        SpawnGuards();
        SpawnInmates();
        

       
       
        Player.onAttack += pAttack;
        SetTarget();
       // target.GetComponent<Renderer>().material = targetMat;
    }
    void OnDisable()
    {
        Player.onAttack -= pAttack;
    }
    void pAttack()
    {
        if (playerCaught)
        {
            EndGame(false);
        }
        else
        {
            EndGame(true);
        }
     
    }


    // Update is called once per frame
    void Update() {
        gameTimer -= Time.deltaTime;
        timerTxt.text = "You have : " + gameTimer.ToString("F0") +  " Seconds Remaning to kill the target";
        if (gameTimer <=0)
        {
            EndGame(false);
        }
        if (dstFromGuards > 2)
        {
            dstFromGuards = 2;
        }
     

        if (endGame)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(0);
            }
        }
        OrderList(true);
    }
   


    private void SpawnInmates()
    {
        int x = rnd.Next(0, numInmates);
        int z = 0;
        int y = 0;
        do
        {
            if (i == numInmatesPerGroup)
            {
                
                y++;
                z = 0;
                numInmatesPerGroup = numInmatesPerGroup + cachInmateNum;
            }
            Vector3 center = spawnPositions[y].transform.position;
          
            Vector3 pos = RandomCircle(center, 1.5f, z  );
            pos.y = 0;
            Quaternion rot = Quaternion.LookRotation(center - pos);
            if (i == x)
            {
                GameObject objIns = Instantiate(targetObj, pos, rot) as GameObject;
                target = objIns.GetComponent<Target>();
                objIns.GetComponent<Target>().myType = AI.TypeOfAI.sketchyTarget;
            }
            else
            {
                Inmate inmateIns = Instantiate(inmate, pos, rot) as Inmate;
                _inmates.Add(inmateIns);
                inmateIns.RndNum = rnd.Next(0, 50);
            }
            i++;
            z++;
          


        } while (i <= numInmates);
    
    }


    Vector3 RandomCircle(Vector3 center, float radius , int x)
    {
        float ang =x * rnd.Next(62,(int)dstFromInmates);
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    public void OrderList(bool guards)
    {
        if (guards)
        {
         _guard = _guard.OrderBy(x => Vector3.Distance(player.transform.position, x.transform.position)).ToList();
            
           
        }
        else
        {
           _inmates = _inmates.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToList();

        }

    }

     
    
    public bool CheckDistanceOthers(Vector3 point)
    {
        if (_inmates.Count > 1)
        {
            _inmates = _inmates.OrderBy(x => Vector3.Distance(point, x.transform.position)).ToList();

        }

        for (int x = 0; x < numInmatesPerGroup; x++)
        {
            if (_inmates.Count > x)
            {
                if (Vector3.Distance(point, _inmates[x].transform.position) < dstFromInmates)
                {
                    return true;
                }
              

            }
          
         
        }
        
        return false;
    }
    private void SpawnGuards()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 dir = transform.position - guardSpots[i].transform.position;
            Quaternion rot = Quaternion.LookRotation(dir.normalized);
            Guard guardIns = Instantiate(guard, guardSpots[i].transform.position, rot) as Guard;
            guardIns.name = "Guard " + i;
            _guard.Add(guardIns.GetComponent<Guard>());
        }
    }

     public void EndGame(bool win)
    {
        text.gameObject.SetActive(true);

        endGame = true;
        if (win)
        {
            text.text = "You Win Press 'R' to Restart";
        }
        else
        {
            text.text = "You Lose Press 'R' to Restart";
        }
     
    }
    void SetTarget()
    {
        foreach (Inmate mate in _inmates)
        {
            mate.Target = target;
        }
    }

}
