using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Queue : MonoBehaviour {
    Queue<GameObject> waterQueue = new Queue<GameObject>();
    Queue<GameObject> foodQueue = new Queue<GameObject>();

    GameObject currentObject;
    static Queue instance;

    private bool isProcessingPath;
	// Use this for initialization
	void Start () {
        instance = this;
	}
	
    public static void Request(string whatToQueue, GameObject whoIsQueuing)
    {
        if (whatToQueue == "Water")
        {
            instance.waterQueue.Enqueue(whoIsQueuing);
           
        }
        if(whatToQueue == "Food")
        {
            instance.foodQueue.Enqueue(whoIsQueuing);
        }
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath)
        {
            if(waterQueue.Count > 0)
                currentObject = waterQueue.Dequeue();
            else if (foodQueue.Count > 0)
               currentObject = foodQueue.Dequeue();

            isProcessingPath = true;
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
