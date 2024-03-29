using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyInvestigateSuperState : EnemyState
{
    public EnemyInvestigateSuperState(Enemy enemy) : base(enemy)
    {
        movementSpeed = enemy.Data.InvestigateMovementSpeed;
    }

    protected bool End_TargetReached { get; private set; }
    protected bool End_PlayerFound { get; private set; }

    private readonly float movementSpeed; // SO

    private Stack<Vector2> pathStack;
    private Vector2 currentTargetNode;

    protected void RefreshPath()
    {
        pathStack = enemy.Pathfinder.FindPathWithBias(enemy.transform.position, enemy.lastKnownPlayerPosition);
        if (pathStack.Any()) currentTargetNode = pathStack.Pop();
        else End_TargetReached = true;
    }

    public override void Enter()
    {
        base.Enter();

        End_TargetReached = false;
        End_PlayerFound = false;

        RefreshPath();
    }

    public override void Update()
    {
        base.Update();

        // Update weapon position
        UpdateWeaponPosition();

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
        if (enemy.IsPlayerVisible) {
            enemy.suspiciousDetection = true;
            End_PlayerFound = true;
        }
    }
}
