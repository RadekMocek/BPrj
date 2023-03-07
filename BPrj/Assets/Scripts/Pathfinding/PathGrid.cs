using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathGrid
{
    // Constant prices for moving between nodes on the grid
    public static readonly int straightPathCost = 10;
    public static readonly int diagonalPathCost = 14;

    // Global world coordinates of path's start and finish
    private Vector2Int startCoordinates;
    private Vector2Int endCoordinates;

    // This pathfinding solution is not achieved by firm sized grid, nodes are added to database gradually
    private Dictionary<Vector2Int, PathNode> nodesDatabase;

    // This list is used as if it was a priority queue, it stores unstable nodes with possible Cost
    // Nodes are sorted by their Combined value: f+h = Cost of getting to the node (Dijkstra) + heuristic function (A*)
    private List<PathNode> priorityQueue;
    
    //
    private PathNode startingNode;
    private PathNode priorityNode;
    private PathNode neighNode;
    private Vector2Int tempVector;
    private Stack<Vector2Int> returnStack;

    public Stack<Vector2Int> FindPath(Vector2 start, Vector2 end)
    {
        // Initialize collections
        returnStack = new Stack<Vector2Int>();
        priorityQueue = new List<PathNode>();
        nodesDatabase = new Dictionary<Vector2Int, PathNode>();

        // Create starting node, add it to nodeDatabase and priorityQueue
        // - Snap start and finish coordinates to the (integer) grid
        startCoordinates = Vector2Int.RoundToInt(start);
        endCoordinates = Vector2Int.RoundToInt(end);
        // - Start has Cost of 0 and should be walkable (caller is standing on it)
        startingNode = new PathNode(startCoordinates, 0, endCoordinates, true);
        nodesDatabase.Add(startCoordinates, startingNode);
        priorityQueue.Add(startingNode);

        // Starting node has Cost of 0, every other node has `impossibleCost` (must be bigger than the longest possible path)
        int impossibleCost = (int)Mathf.Pow(Vector2Int.Distance(startCoordinates, endCoordinates) * diagonalPathCost, 2);

        // Repeat this process until priorityQueue is empty or until end node is found
        while (true) {
            
            if (!priorityQueue.Any()) {
                // Failure – no more nodes in priorityQueue
                returnStack.Push(startCoordinates);
                return returnStack;
            }

            // Dequeue (find node with minimum f+h value)
            priorityNode = priorityQueue[0];
            for (int i = 1; i < priorityQueue.Count; i++) {
                if (priorityQueue[i].Combined < priorityNode.Combined) priorityNode = priorityQueue[i];
            }

            if (priorityNode.Coordinates == endCoordinates) {
                // Success – (dequeued) priorityNode is the end node

                PathNode pathNode = priorityNode;

                while (pathNode.Precursor != null) {
                    returnStack.Push(pathNode.Coordinates);
                    pathNode = pathNode.Precursor;
                }

                return returnStack;
            }
            
            priorityQueue.Remove(priorityNode);
            priorityNode.Stable = true; // Mark priorityNode as stable so we dont't enqueue it again later (avoid going in cirles)

            // Get values from priorityNode
            int x = priorityNode.Coordinates.x;
            int y = priorityNode.Coordinates.y;
            int cost = priorityNode.Cost;

            // Foreach neighbour
            for (int horizontal = -1; horizontal <= 1; horizontal++) {
                for (int vertical = -1; vertical <= 1; vertical++) {

                    if (horizontal == 0 && vertical == 0) continue; // Do not check itself

                    tempVector.Set(x + horizontal, y + vertical); // Neighbour coordinates

                    // Add neighbor to nodeDatabase or load it if it's already in nodeDatabase
                    if (!nodesDatabase.ContainsKey(tempVector)) {
                        neighNode = new PathNode(tempVector, impossibleCost, endCoordinates, true); //TODO: WALKABLE
                        nodesDatabase.Add(tempVector, neighNode);
                    }
                    else {
                        neighNode = nodesDatabase[tempVector];
                    }

                    int pathCost = ((horizontal == vertical || horizontal == -vertical) && horizontal != 0) ? diagonalPathCost : straightPathCost;

                    // Update neigbour's Cost, Precursor, and add it to priorityQueue if it's unstable, walkable, and its current Cost is more expensive than going from the priorityNode
                    if (!neighNode.Stable && neighNode.Walkable && cost + pathCost < neighNode.Cost) {
                        neighNode.SetCost(cost + pathCost);
                        neighNode.Precursor = priorityNode;
                        priorityQueue.Add(neighNode);
                    }

                }
            }

        } // End of while
    }
}
