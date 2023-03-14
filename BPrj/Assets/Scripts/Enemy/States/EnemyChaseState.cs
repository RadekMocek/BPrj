using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(Enemy enemy) : base(enemy)
    {
    }

    protected bool End_PlayerLost { get; private set; }

    private readonly float movementSpeed = 4.7f;

    private Stack<Vector2Int> pathStack;
    private Vector2Int currentTargetNode;

    private Vector2 currentPlayerPosition;
    private Vector2 previousPlayerPosition;

    private void RefreshPath(Vector2 target)
    {
        // We want path lead to the tile a little in front of the target to prevent target tile being out of bounds
        //Vector2 end = currentPlayerPosition + 1.2f * ((Vector2)enemy.transform.position - target).normalized;

        // Fill the stack and get the first target node
        pathStack = enemy.Pathfinder.FindPathWithBias(enemy.transform.position, target);
        if (pathStack.Any()) {
            currentTargetNode = pathStack.Pop();
        }
    }

    public override void Enter()
    {
        base.Enter();

        End_PlayerLost = false;

        currentPlayerPosition = enemy.EnemyManager.GetPlayerPosition();
        previousPlayerPosition = currentPlayerPosition;

        RefreshPath(currentPlayerPosition);
    }

    public override void Update()
    {
        base.Update();

        enemy.FaceThePlayer(false);
        enemy.MovementToAnimation();

        bool isPlayerVisible = enemy.IsPlayerVisible(true);

        if (isPlayerVisible) {
            currentPlayerPosition = enemy.EnemyManager.GetPlayerPosition();

            if (currentPlayerPosition != previousPlayerPosition) {
                previousPlayerPosition = currentPlayerPosition;
                RefreshPath(currentPlayerPosition);
            }
        }
        else {
            End_PlayerLost = true;
        }

        // Move in the direction of the target path node until enemy is close enough
        if (Vector2.Distance(enemy.transform.position, currentTargetNode) > 0.1f) {
            var movementDirection = (currentTargetNode - (Vector2)enemy.transform.position).normalized;
            enemy.RB.velocity = movementDirection * movementSpeed;
        }
        // Switch to next path node if enemy is close to the target path node
        else if (pathStack.Any()) {
            currentTargetNode = pathStack.Pop();
        }
    }
}
