using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(Enemy enemy) : base(enemy)
    {
    }

    private Vector2[] patrolPoints;
    private int nPatrolPoints;
    private int patrolPointIndex;
    private Vector2 currentTargetPoint;

    private Stack<Vector2Int> pathStack;
    private Vector2Int currentTargetNode;

    private void NextPatrolPoint()
    {
        patrolPointIndex = (patrolPointIndex == nPatrolPoints - 1) ? 0 : patrolPointIndex + 1;
        currentTargetPoint = patrolPoints[patrolPointIndex];

        pathStack = enemy.Pathfinder.FindPath(enemy.transform.position, currentTargetPoint);
        if (pathStack.Any()) {
            currentTargetNode = pathStack.Pop();
        }

    }

    public override void Enter()
    {
        base.Enter();

        patrolPoints = enemy.GetPatrolPoints();
        nPatrolPoints = patrolPoints.Length;

        if (nPatrolPoints == 0) {
            Debug.LogError("Enemy has no patrol points.");
            currentTargetPoint = Vector2.zero;
            return;
        }

        patrolPointIndex = -1;
        NextPatrolPoint();
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(enemy.transform.position, currentTargetNode) > 0.1f) {
            enemy.RB.velocity = (currentTargetNode - (Vector2)enemy.transform.position).normalized * 10;
        }
        else if (pathStack.Any()) {
            currentTargetNode = pathStack.Pop();
        }
        else {
            NextPatrolPoint();
        }
        
    }

}
