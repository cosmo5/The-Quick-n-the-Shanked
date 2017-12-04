using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
public class Pathfinding : MonoBehaviour {
    Grid grid;
    PathRequestManager requestManager;
    GameManager gm;
    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }
   
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath( Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
            
                closedSet.Add(currentNode);
           
                if (currentNode == targetNode)
                {
                    sw.Stop();
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (Physics.CheckSphere(neighbour.worldPos, grid.nodeRadius * gm.nodeRadiusMultiplyer, gm.inmateMask) && !gm.yardOver)
                    { 
                        neighbour.movementPenalty = 50;
                    }
                    else
                    {
                        foreach (Node neighbour2 in grid.GetNeighbours(neighbour))
                        {
                            if (neighbour2.movementPenalty > 25)
                            {
                                neighbour.movementPenalty = 25;
                            }
                            else
                            {
                                 neighbour.movementPenalty = 0;
                            }
                        }
                       
                    }
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                
                    //the value for the new movement cost travelling to one of the neighbour nodes
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    //Check if the new path to neighbour is shorter or if the neighbour is not in the open set 
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        //Set fCost of neighbour
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);

                        //set parent of neighbour to the node
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                        neighbour.movementPenalty = 0;
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
          waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishProcessingPath(waypoints, pathSuccess);
    }

    //Used for when we find the path to the end node
    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        //Create new list to store the path as we get the nodes
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
         Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

        
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);

            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPos);

            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }
    int GetDistance(Node nodeA, Node nodeB)
    {
        //equation for finding distance from end node = 14y + 10(x-y)
        // diagonal movement costs 14 and each horizontal or verticle movement costs 10 
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX >distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {

        return 14 * distX + 10 * (distY - distX);
        } 
        
    }
}
