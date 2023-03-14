public class Enemy1InvestigateState : EnemyInvestigateState
{
    public Enemy1InvestigateState(Enemy enemy) : base(enemy)
    {
        enemy1 = enemy as Enemy1;
    }

    private readonly Enemy1 enemy1;
}
