public class Enemy1InvestigateSuspiciousState : EnemyInvestigateSuspiciousState
{
    public Enemy1InvestigateSuspiciousState(Enemy enemy) : base(enemy)
    {
        enemy1 = enemy as Enemy1;
    }

    private readonly Enemy1 enemy1;

    public override void Update()
    {
        base.Update();

        if (End_PlayerFound) {
            enemy1.ChangeState(enemy1.DetectingState);
        }
        else if (End_TargetReached) {
            enemy1.ChangeState(enemy1.LookAroundState);
        }
    }
}
