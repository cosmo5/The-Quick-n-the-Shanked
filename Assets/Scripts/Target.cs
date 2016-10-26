using UnityEngine;
using System.Collections;

public class Target : Inmate {
    public bool stayLockedToPlayer;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void Think(int x, Vector3 randomDir, Quaternion startRot)
    {
        base.Think(x, randomDir, startRot);
      
        switch (myType)

        {
            case TypeOfAI.sketchyTarget:
                if (x > 0 && x < 20 || stayLockedToPlayer)
                {
                    if (Vector3.Distance(transform.position, player.transform.position) < 2)
                    {
                        //Freak Out
                        Rotate(player.transform.position - transform.position, false, transform.rotation);
                        stayLockedToPlayer = true;
                    }
                    if (Vector3.Distance(transform.position, player.transform.position) > 5)
                    {
                        stayLockedToPlayer = false;
                    }
                }
                break;
            case TypeOfAI.targetInGang:
                break;

            default:
                break;
        }
        
    }
}
