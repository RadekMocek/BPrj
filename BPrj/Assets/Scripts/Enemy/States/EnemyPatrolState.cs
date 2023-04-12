using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(Enemy enemy) : base(enemy)
    {
        patrolPointIndex = -1; // (`NextPatrolPoint()` adds one to the `patrolPointIndex`)

        movementSpeed = enemy.Data.PatrolMovementSpeed;
    }

    protected bool End_PlayerVisible { get; private set; }
    protected bool End_PatrolPointReached { get; private set; }

    private readonly float movementSpeed; // SO

    private Vector2[] patrolPoints;     // Enemy will patrol from point to point
    private int nPatrolPoints;          // Length of `patrolPoints`
    private int patrolPointIndex;       // Index of the target point where enemy is currently heading
    private Vector2 currentTargetPoint; // and coordinates of that point

    private Stack<Vector2> pathStack;    // Stack of node coordinates that creates a path to the target point
    private Vector2 currentTargetNode;   // Coordinates of the target node where enemy is currently heading

    // Called when enemy reaches `currentTargetPoint`; switches to the next patrol point
    private void NextPatrolPoint()
    {
        // Choose the next target point
        patrolPointIndex = (patrolPointIndex + 1) % nPatrolPoints;
        currentTargetPoint = patrolPoints[patrolPointIndex];

        // Fill the stack and get the first target node
        pathStack = enemy.Pathfinder.FindPath(enemy.transform.position, currentTargetPoint);
        if (pathStack.Any()) {
            currentTargetNode = pathStack.Pop();
        }
    }

    public override void Enter()
    {
        base.Enter();

        End_PlayerVisible = false;
        End_PatrolPointReached = false;

        enemy.suspiciousDetection = false;

        enemy.ChangeViewConeColor(Color.green);

        patrolPoints = enemy.GetPatrolPoints();
        nPatrolPoints = patrolPoints.Length;

        if (nPatrolPoints == 0) {
            Debug.LogError("Enemy has no patrol points.");
            currentTargetPoint = Vector2.zero;
            return;
        }

        NextPatrolPoint();
    }

    public override void Update()
    {
        base.Update();

        // Update weapon position
        UpdateWeaponPosition();

        // Decrease red cone radius over time
        enemy.UpdateDecreaseViewConeRedRadius();

        //
        if (enemy.IsPlayerVisible) {
            patrolPointIndex--;
            End_PlayerVisible = true;
        }

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
        // If there are no path nodes left, enemy has reached the target patrol point
        else {
            End_PatrolPointReached = true;
        }
    }

}
