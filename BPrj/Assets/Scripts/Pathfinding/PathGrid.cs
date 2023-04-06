using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathGrid
{
    // == Pathfinding ==
    // Config
    private readonly bool isDiagonalMovementAllowed = true;

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
    
    // Temp vars
    private PathNode startingNode;
    private PathNode priorityNode;
    private PathNode neighNode;
    private Vector2Int tempVector;
    private Vector2Int tempVectorDiagonalCheckX;
    private Vector2Int tempVectorDiagonalCheckY;
    private Stack<Vector2> returnStack;

    // Pathfinding, returns Stack of coordinates that make path from `start` to `end`
    public Stack<Vector2> FindPath(Vector2 start, Vector2 end)
    {
        // Initialize collections
        returnStack = new Stack<Vector2>();
        priorityQueue = new List<PathNode>();
        nodesDatabase = new Dictionary<Vector2Int, PathNode>();

        // Create starting node, add it to nodeDatabase and priorityQueue
        // - Snap start and finish coordinates to the (integer) grid
        startCoordinates = Vector2Int.FloorToInt(start);
        endCoordinates = Vector2Int.FloorToInt(end);
        // - Terminate if end unreachable
        if (!IsWalkable(endCoordinates)) {
            returnStack.Push(startCoordinates + tileCenterAddition);
            return returnStack;
        }
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
                returnStack.Push(startCoordinates + tileCenterAddition);
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
                    returnStack.Push(pathNode.Coordinates + tileCenterAddition);
                    pathNode = pathNode.Precursor;
                }
                return returnStack;
            }
            
            priorityQueue.Remove(priorityNode);
            priorityNode.Stable = true; // Mark priorityNode as stable so we don't enqueue it again later (avoid going in circles)

            // Get values from priorityNode
            int x = priorityNode.Coordinates.x;
            int y = priorityNode.Coordinates.y;
            int cost = priorityNode.Cost;

            // Foreach neighbour
            for (int horizontal = -1; horizontal <= 1; horizontal++) {
                for (int vertical = -1; vertical <= 1; vertical++) {
                    if (horizontal == 0 && vertical == 0) continue; // Do not check itself

                    bool isNeighbourDiagonal = (horizontal != 0 && vertical != 0);

                    if (!isDiagonalMovementAllowed && isNeighbourDiagonal) continue;

                    tempVector.Set(x + horizontal, y + vertical); // Neighbour coordinates
                    
                    if (isDiagonalMovementAllowed && isNeighbourDiagonal) {
                        tempVectorDiagonalCheckX.Set(tempVector.x, y);
                        tempVectorDiagonalCheckY.Set(x, tempVector.y);
                        if (!IsWalkable(tempVectorDiagonalCheckX) || !IsWalkable(tempVectorDiagonalCheckY)) continue;
                    }

                    // Add neighbour to nodeDatabase or load it if it's already in nodeDatabase
                    if (!nodesDatabase.ContainsKey(tempVector)) {
                        neighNode = new PathNode(tempVector, impossibleCost, endCoordinates, IsWalkable(tempVector));
                        nodesDatabase.Add(tempVector, neighNode);
                    }
                    else {
                        neighNode = nodesDatabase[tempVector];
                    }

                    int pathCost = (isNeighbourDiagonal) ? diagonalPathCost : straightPathCost;

                    // Update neighbours Cost, Precursor, and add it to priorityQueue if it's unstable, walkable, and its current Cost is more expensive than going from the priorityNode
                    if (!neighNode.Stable && neighNode.Walkable && cost + pathCost < neighNode.Cost) {
                        neighNode.SetCost(cost + pathCost);
                        neighNode.Precursor = priorityNode;
                        priorityQueue.Add(neighNode);
                    }
                }
            } // End of foreach neighbour
        } // End of while
    } // End of method

    // == Walkable ==
    public static LayerMask unwalkableLayer;
    public static Tilemap floorTilemap;

    private static readonly Vector2 tileCheckDiagonalRadius = new(-0.31f, 0.31f);
    private static readonly Vector2 tileCenterAddition = new(0.5f, 0.5f);

    public static bool IsWalkable(Vector2Int coordinates)
    {
        if (floorTilemap == null) return false;

        bool tilemapOk = floorTilemap.HasTile((Vector3Int)coordinates);

        Physics2D.queriesHitTriggers = false;
        bool layerOk = !Physics2D.OverlapArea(coordinates + tileCenterAddition + tileCheckDiagonalRadius, coordinates + tileCenterAddition - tileCheckDiagonalRadius, unwalkableLayer);
        Physics2D.queriesHitTriggers = true;

        return tilemapOk && layerOk;
    }

    // == Pathfinding with bias ==
    private Vector2Int endCoordinatesWithBias;
    // If end tile is unreachable then try alternative end tiles
    public Stack<Vector2> FindPathWithBias(Vector2 start, Vector2 end)
    {
        endCoordinates = Vector2Int.FloorToInt(end);

        if (IsWalkable(endCoordinates)) {
            return FindPath(start, end);
        }

        ///*
        bool isEndAboveStart = (end.y > start.y);
        for (int y = (isEndAboveStart) ? -1 : 1; (isEndAboveStart && y <= 1) || (!isEndAboveStart && y >= -1); y += (isEndAboveStart) ? 1 : -1) {
        /**/
        //for (int y = -1; y <= 1; y++) {
            for (int x = -1; x <= 1; x++) {
                if (y == 0 && x == 0) continue;
                endCoordinatesWithBias.Set(endCoordinates.x + x, endCoordinates.y + y);
                if (IsWalkable(endCoordinatesWithBias)) {
                    return FindPath(start, endCoordinatesWithBias);
                }
            }
        }

        return new Stack<Vector2>();
    }
}
