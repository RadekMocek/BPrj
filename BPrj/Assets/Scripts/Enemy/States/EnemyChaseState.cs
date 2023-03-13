using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(Enemy enemy) : base(enemy)
    {
    }

    private readonly float movementSpeed = 4.7f;

    private Stack<Vector2Int> pathStack;
    private Vector2Int currentTargetNode;

    private Vector2 currentPlayerPosition;
    private Vector2 previousPlayerPosition;

    private void RefreshPath(Vector2 end)
    {
        // Fill the stack and get the first target node
        pathStack = enemy.Pathfinder.FindPath(enemy.transform.position, end);
        if (pathStack.Any()) {
            currentTargetNode = pathStack.Pop();
        }
    }

    public override void Enter()
    {
        base.Enter();

        currentPlayerPosition = enemy.EnemyManager.GetPlayerPosition();
        previousPlayerPosition = currentPlayerPosition;

        RefreshPath(currentPlayerPosition);
    }

    public override void Update()
    {
        base.Update();

        currentPlayerPosition = enemy.EnemyManager.GetPlayerPosition();

        if (currentPlayerPosition != previousPlayerPosition) {
            previousPlayerPosition = currentPlayerPosition;
            RefreshPath(currentPlayerPosition);
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
    }
}
