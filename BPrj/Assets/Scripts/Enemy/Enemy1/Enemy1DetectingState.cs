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
            enemy1.ChangeState(enemy1.PatrolState);
        }
        else if (End_PlayerSpotted) {
            enemy1.ChangeState(enemy1.ChaseState);
        }
    }
}
