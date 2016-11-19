using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
public class GameManager : MonoBehaviour {

    public enum TypeOfDay
    {
        Hot,
        Warm,
        JustRight,
        Cold,
        Rainey,
        Storming,
    };
    public TypeOfDay typeOfDay = TypeOfDay.JustRight;
    public GameObject player;                   /// 
    public GameObject[] guardSpots;             ///
    public GameObject targetObj;                ///
    public Guard guard;                         ///References to objects for instantiating 
    public Inmate inmate;                       ///
    public Target target;                      ///

    public float[] yardHours = new float[2];
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

    public float startHour;
    public float worldTimeHours;
    public float worldTimeMins;
    private float startTime;
    public Text worldTimer;                     // The World Timer
    public float timeSpeedMultiplyer;
    public Text text;                       //  Text objects to show the
    public Text timerTxt;                   //  timer and game over text

    public GameObject[] mapKeyPoints;

    public GameObject spawnObj;
    public GameObject[] spawnPositions;
    public int spawnCount;
    public int numInmatesPerGroup;
    private int cachInmateNum;
    public float nodeRadiusMultiplyer;
    public LayerMask inmateMask;

    public bool yardOver;
    public delegate void YardTimeOver();
    public static event YardTimeOver yardTimeOver;


    public delegate void NewHour();
    public static event NewHour newHour;
    public bool drawGizmosPath;
    public float dayMultiplyer;
    private bool dayVarsSet;
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
        startTime = Time.time;
        worldTimeHours = startHour;
       
       
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
        worldTimeMins = (Time.time - startTime) * timeSpeedMultiplyer;
        if (worldTimeMins > 60)
        {
            if (newHour != null)
            {
                newHour();
            }
            worldTimeHours += 1;
            if (worldTimeHours >= yardHours[1])
            {
                if (yardTimeOver !=null)
                {
                    yardTimeOver();
                    yardOver = true;
                }
            }
            startTime = Time.time;
            worldTimeMins = 0;
        }
        if(worldTimeMins > 10)
          worldTimer.text = "Time: " +(    worldTimeHours ).ToString("F0") + ":" + worldTimeMins.ToString("F0");
        else
            worldTimer.text = "Time: " + (worldTimeHours).ToString("F0") + ":" + "0" + worldTimeMins.ToString("F0");


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
        OrderList(true, player.transform.position);
        if (!dayVarsSet)
        {
            switch (typeOfDay)
            {
                case TypeOfDay.Hot:
                    dayMultiplyer = 3;
                    break;
                case TypeOfDay.Warm:
                    dayMultiplyer = 2;
                    break;
                case TypeOfDay.JustRight:
                    dayMultiplyer = 1;
                    break;
                case TypeOfDay.Cold:
                    dayMultiplyer = 0.8f;
                    break;
                case TypeOfDay.Rainey:
                    dayMultiplyer = 0.5f;
                    break;
                case TypeOfDay.Storming:
                    dayMultiplyer = 0.3f;
                    break;
                default:
                    break;
            }
            dayVarsSet = true;  
        }
       
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
          
            Vector3 pos = RandomCircle(center, 1, z  );
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
        float ang = x * rnd.Next(62,(int)dstFromInmates);
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    public void OrderList(bool guards, Vector3 pos)
    {
        if (guards)
        {
         _guard = _guard.OrderBy(x => Vector3.Distance(pos, x.transform.position)).ToList();
            
           
        }
        else
        {
           _inmates = _inmates.OrderBy(x => Vector3.Distance(pos, x.transform.position)).ToList();

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
