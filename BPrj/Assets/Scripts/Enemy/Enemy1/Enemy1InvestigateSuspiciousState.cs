public class Enemy1InvestigateSuspiciousState : EnemyInvestigateSuspiciousState
{
    public Enemy1InvestigateSuspiciousState(Enemy enemy) : base(enemy)
    {
        enemy1 = enemy as Enemy1;
    }

    private readonly Enemy1 enemy1;
}
