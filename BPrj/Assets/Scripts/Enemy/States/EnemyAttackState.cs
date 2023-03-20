using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(Enemy enemy) : base(enemy)
    {
    }

    private readonly int swingCircularSectorAngle = 80;

    private float angle;
    private float endingAngle;

    public override void Enter()
    {
        base.Enter();

        //endingAngle = (int)(Mathf.Rad2Deg * Mathf.Atan2(playerToCursorDirection.y, playerToCursorDirection.x));
        angle = endingAngle - swingCircularSectorAngle;
    }

    public override void Update()
    {
        base.Update();

        enemy.RB.velocity = Vector2.zero;
    }
}
