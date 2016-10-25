using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public delegate void PlayerAttack();
    public static event PlayerAttack onAttack;
    private GameObject target;

    public float moveSpeed;
    public float maxSpeed;
    public float attackDst;
    Rigidbody rigi;
    private GameManager gm;
	// Use this for initialization
	void Start () {
        gm = FindObjectOfType<GameManager>();
        target = gm.Target().gameObject;
        rigi = GetComponent<Rigidbody>();

    }
    void ClampPos()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, gm.xMin - 1, gm.xMax + 1);
        pos.z = Mathf.Clamp(pos.z, gm.zMin - 1, gm.Zmax + 1);

        transform.position = pos;
    }
	void Update()
    {
        ClampPos(); 
        if (Vector3.Distance(transform.position, target.transform.position) < attackDst)
        {
            if (Input.GetButtonDown("Attack"))
            {
                if (onAttack != null)
                {
                    onAttack();
                }
            }

        }
    }
	// Update is called once per frame
	void FixedUpdate () {

        float x = Input.GetAxisRaw("Horizontal") * moveSpeed;
        float z = Input.GetAxisRaw("Vertical") * moveSpeed;
      
        rigi.AddForce(new Vector3(x, 0, z)  * Time.deltaTime, ForceMode.Impulse );
        if(rigi.velocity.magnitude >maxSpeed)
             rigi.velocity = rigi.velocity.normalized * maxSpeed;


       
	}


}
