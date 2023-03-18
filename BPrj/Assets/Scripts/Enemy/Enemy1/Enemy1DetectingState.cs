public class Enemy1DetectingState : EnemyDetectingState
{
    public Enemy1DetectingState(Enemy enemy) : base(enemy)
    {
        enemy1 = enemy as Enemy1;
    }

    private readonly Enemy1 enemy1;

    public override void Update()
    {
        base.Update();

        if (End_PlayerLost) {
            if (enemy.suspiciousDetection || DetectionLengthWhenPlayerLost >= enemy1.viewDistance / 2) {
                enemy1.ChangeState(enemy1.InvestigateSuspiciousState);
            }
            else {
                enemy1.ChangeState(enemy1.PatrolState);
            }
        }
        else if (End_PlayerSpotted) {
            enemy1.ChangeState(enemy1.ChaseState);
        }
    }
}
