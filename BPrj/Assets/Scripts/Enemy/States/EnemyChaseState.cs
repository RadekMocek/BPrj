using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(Enemy enemy) : base(enemy)
    {
        movementSpeed = enemy.Data.ChaseMovementSpeed;
    }

    protected bool End_PlayerLost { get; private set; }
    protected bool End_PlayerClose { get; private set; }

    private readonly float movementSpeed; // SO

    private Stack<Vector2> pathStack;
    private Vector2 currentTargetNode;

    private Vector2 currentPlayerPosition;
    private Vector2 previousPlayerPosition;

    private void RefreshPath(Vector2 target)
    {
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
        End_PlayerClose = false;

        currentPlayerPosition = enemy.EnemyManager.GetPlayerPosition();
        previousPlayerPosition = currentPlayerPosition;
        
        RefreshPath(currentPlayerPosition);

        // Full red view cone
        enemy.ChangeViewConeColor(Color.yellow);
        enemy.ChangeViewConeRedRadius(enemy.ViewDistance);
    }

    public override void Update()
    {
        base.Update();

        // Update weapon position
        UpdateWeaponPosition();

        // Look directly at the player but change animation according to movement (RB velocity)
        enemy.FaceThePlayer(false);
        enemy.MovementToAnimation();

        // Close attack
        if (enemy.IsPlayerVisibleClose()) {
            End_PlayerClose = true;
        }

        // Follow player (if visible) or transition to another state
        if (enemy.IsPlayerVisible) {
            currentPlayerPosition = enemy.EnemyManager.GetPlayerPosition();

            if (currentPlayerPosition != previousPlayerPosition) {
                previousPlayerPosition = currentPlayerPosition;
                RefreshPath(currentPlayerPosition);
            }
        }
        // If player not visible and no more nodes in path, transition to another state
        else if (!pathStack.Any()) {
            enemy.lastKnownPlayerPosition = enemy.EnemyManager.GetPlayerPositionWalkable();
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
