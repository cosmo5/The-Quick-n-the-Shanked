using UnityEngine;
using System.Collections;

public class Guard : AI {
    public float maxAngle;
    public LayerMask playerLayer = new LayerMask();
    private bool playerCaught;
    private float cacheTimer;

 
    // Use this for initialization
   protected override void Start () {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
        Player.onAttack += PlayerAttack;
        decisionTimer = Random.Range(8, 12);
        cacheTimer = decisionTimer;
	}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (playerCaught)
        {
            RequestPath(transform.position, player.transform.position);
        }
        if (Vector3.Distance(transform.position, player.transform.position) < 2)
        {
            Rotate(player.transform.position - transform.position, false, transform.rotation);
        }
        else
        {
            Think(rndNumber, randomDir, _startRot, false); 
        }

     

      
	}
    public void Snitched()
    {
        playerCaught = true;
        //FindObjectOfType<GameManager>().EndGame(false);
        FindObjectOfType<GameManager>().playerCaught = true;
    }
    void OnDisable()
    {
        Player.onAttack -= PlayerAttack;
    }
    void PlayerAttack()
    {
        if (playerInSight)
        {
            Debug.Log("You Got Caught");
            playerCaught = true;
            FindObjectOfType<GameManager>().playerCaught = true;
            FindObjectOfType<GameManager>().EndGame(false);
        }
    }
    void TargetScared()
    {
        Snitched();
    }
    void OnTriggerStay(Collider c)
    {
        if (c.tag == "Player")
        {
            
            playerInSight = false;
            Vector3 direction = c.transform.position  - transform.position  ;

            Look(direction, moving);
        }
        if (c.tag == "Inmate")
        {
            if (c.GetComponent<Inmate>().narcing)
            {
                if (Vector3.Distance(transform.position, c.transform.position)< 1)
                {
                    Snitched();
                    c.GetComponent<Inmate>().guardToNarcTo = null;
                }

            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Player")
        {
            playerInSight = false;
        }
    }
}
