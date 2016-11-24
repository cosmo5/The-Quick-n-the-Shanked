using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Water : MonoBehaviour {
    Queue<AI> waterQueue1 = new Queue<AI>();
    public int queueCount;
    public AI curr1;
   public List<AI> _ai = new List<AI>();
    public bool beingUsed;
    // Use this for initialization
    void Start () {
	    
	}
    public void RequestWater(AI obj)
    {
        waterQueue1.Enqueue(obj);
     
    }
 
   void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Inmate")
        {
            _ai.Add(other.GetComponent<AI>());
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Inmate")
        {
            _ai.Remove(other.GetComponent<AI>());
            
        }
    }

    // Update is called once per frame
    void Update () {
        if (curr1 != null)
        {
            curr1.GainWater(10 * Time.deltaTime);
            if (!curr1.GettingWater)
            {
                curr1 = null;
            }
        }
        else
        {
            if (waterQueue1.Count > 0)
            {
                curr1 = waterQueue1.Dequeue();
                beingUsed = true;
            }
            else
            {
                beingUsed = false;
                

            }
        }
	}

   
}
