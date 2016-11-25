using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GetComponent<Inmate_Entity>().FSM.ChangeState(MovingState.Instance);

        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            GetComponent<Inmate_Entity>().FSM.ChangeState(DrinkingState.Instance);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            GetComponent<Inmate_Entity>().FSM.ChangeState(IdleState.Instance);
        }
    }
}
