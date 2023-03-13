public class Enemy1PatrolState : EnemyPatrolState
{
    public Enemy1PatrolState(Enemy enemy) : base(enemy)
    {
        enemy1 = enemy as Enemy1;
    }

    private readonly Enemy1 enemy1;

    public override void Update()
    {
        base.Update();

        if (enemy1.IsPlayerVisible()) {
            enemy1.ChangeState(enemy1.DetectingState);
        }
    }
}
