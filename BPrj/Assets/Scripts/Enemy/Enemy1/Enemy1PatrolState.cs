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

        if (End_PlayerVisible) {
            enemy1.ChangeState(enemy1.DetectingState);
        }
        else if (End_PatrolPointReached) {
            enemy1.ChangeState(enemy1.LookAroundState);
        }
    }
}
