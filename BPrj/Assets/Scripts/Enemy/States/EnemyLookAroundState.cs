using UnityEngine;

public class EnemyLookAroundState : EnemyState
{
    public EnemyLookAroundState(Enemy enemy) : base(enemy)
    {
    }

    private readonly float rotationPauseDuration = 1;
    
    private float lastRotationTime;

    public override void Enter()
    {
        base.Enter();

        lastRotationTime = enterTime;
    }

    public override void Update()
    {
        base.Update();

        enemy.RB.velocity = Vector2.zero;

        if (Time.time > lastRotationTime + rotationPauseDuration) {
            lastRotationTime = Time.time;

        }
    }
}
