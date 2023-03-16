using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyInvestigateSuperState : EnemyState
{
    public EnemyInvestigateSuperState(Enemy enemy) : base(enemy)
    {
    }

    protected bool End_TargetReached { get; private set; }
    protected bool End_PlayerFound { get; private set; }

    private readonly float movementSpeed = 4.7f;

    private Stack<Vector2Int> pathStack;
    private Vector2 lastKnownPlayerPosition;
    private Vector2Int currentTargetNode;

    public override void Enter()
    {
        base.Enter();

        End_TargetReached = false;
        End_PlayerFound = false;

        lastKnownPlayerPosition = enemy.lastKnownPlayerPosition;

        pathStack = enemy.Pathfinder.FindPathWithBias(enemy.transform.position, lastKnownPlayerPosition);
        if (pathStack.Any()) currentTargetNode = pathStack.Pop();
    }

    public override void Update()
    {
        base.Update();

        // Move in the direction of the target path node until enemy is close enough
        if (Vector2.Distance(enemy.transform.position, currentTargetNode) > 0.1f) {
            var movementDirection = (currentTargetNode - (Vector2)enemy.transform.position).normalized;
            enemy.DirectionToFacingDirectionAndAnimation(movementDirection);
            enemy.RB.velocity = movementDirection * movementSpeed;
        }
        // Switch to next path node if enemy is close to the target path node
        else if (pathStack.Any()) {
            currentTargetNode = pathStack.Pop();
        }
        // If there are no path nodes left, enemy has reached its destination => switch to a different state
        else {
            End_TargetReached = true;
            enemy.RB.velocity = Vector2.zero;
        }

        // Switch to a different state immediately if the player is spotted
        if (enemy.IsPlayerVisible()) {
            End_PlayerFound = true;
        }
    }
}
