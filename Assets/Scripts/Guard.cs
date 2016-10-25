using UnityEngine;
using System.Collections;

public class Guard : MonoBehaviour {
    public float maxAngle;
    public GameObject player;
    public bool playerInSight = false;
    public LayerMask playerLayer = new LayerMask();
    public float fovAngle = 90;
    public float moveSpeed;
    private bool playerCaught;
    NavMeshAgent nma;
    
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        Player.onAttack += PlayerAttack;

	}

    // Update is called once per frame
    void Update () {
        if (playerCaught)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position , moveSpeed * Time.deltaTime);

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
        }
    }
    void OnTriggerStay(Collider c)
    {
        if (c.tag == "Player")
        {
            playerInSight = false;
            Vector3 direction = c.transform.position  - transform.position  ;
   
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < fovAngle * 0.5f)
            {
         
                RaycastHit hit;
               
                if (Physics.Raycast(transform.GetChild(0).position , direction.normalized, out hit, 30))
                {
                    if (hit.transform.tag == "Player")
                    {
                        playerInSight = true;
                        Debug.DrawLine(transform.position, hit.point, Color.red);

                    }

                }
                else
                {
                    playerInSight = false;
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
