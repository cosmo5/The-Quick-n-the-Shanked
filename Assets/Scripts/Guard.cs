using UnityEngine;
using System.Collections;

public class Guard : AI {
    public float maxAngle;
    public LayerMask playerLayer = new LayerMask();
    private bool playerCaught;
    private float cacheTimer;

  
 
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        Player.onAttack += PlayerAttack;
        decisionTimer = Random.Range(8, 12);
        cacheTimer = decisionTimer;
	}

    // Update is called once per frame
    void Update () {
        if (playerCaught)
        {
            Move(player.transform.position - transform.position);
        }
        if (Vector3.Distance(transform.position, player.transform.position) < 5)
        {
            Rotate(player.transform.position - transform.position, false, transform.rotation);
        }
        else
        {
            Think(rndNumber, randomDir, _startRot); 
        }

        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0)
        {
            rndNumber = rnd.Next(0,100);
            randomDir = Random.insideUnitSphere * 10;
            randomDir.Normalize();
            _startRot = transform.rotation;
            decisionTimer = cacheTimer;
        }

      
	}
    public void Snitched()
    {
        playerCaught = true;
        FindObjectOfType<GameManager>().EndGame(false);
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
    void OnTriggerStay(Collider c)
    {
        if (c.tag == "Player")
        {
            
            playerInSight = false;
            Vector3 direction = c.transform.position  - transform.position  ;

            Look(direction);
        }
        if (c.tag == "Inmate")
        {
            if (c.GetComponent<Inmate>().narcing)
            {
                Snitched();
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
