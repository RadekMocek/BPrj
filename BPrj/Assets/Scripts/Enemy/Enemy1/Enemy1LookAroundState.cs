public class Enemy1LookAroundState : EnemyLookAroundState
{
    public Enemy1LookAroundState(Enemy enemy) : base(enemy)
    {
        enemy1 = enemy as Enemy1;
    }

    private readonly Enemy1 enemy1;
}
