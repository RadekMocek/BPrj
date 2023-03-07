using UnityEngine;

public class PathNode
{
    public Vector2Int Coordinates { get; private set; }

    public int Cost { get; private set; } // Price of getting to this node from start

    private readonly int heuristic; // Value of heuristic function (euclidean distance * diagonal path cost)

    public int Combined { get; private set; } // Cost + heuristic

    public bool Walkable { get; private set; } // Can we walk on this node? false if there are obstacles on node's coordinates

    public bool Stable { get; set; } // True if the cheapest Cost was found

    public PathNode Precursor { get; set; } // Previous node on the cheapest path to this node

    public PathNode(Vector2Int coordinates, int cost, Vector2Int end, bool walkable)
    {
        this.Coordinates = coordinates;

        this.Cost = cost;

        this.heuristic = (int)Vector2Int.Distance(coordinates, end) * PathGrid.diagonalPathCost;

        this.Combined = cost + heuristic;

        this.Walkable = walkable;

        this.Stable = false;

        this.Precursor = null;
    }

    public void SetCost(int value)
    {
        this.Cost = value;

        this.Combined = value + heuristic;
    }

}
