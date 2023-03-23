public class Enemy1KnockbackState : EnemyKnockbackState
{
    public Enemy1KnockbackState(Enemy enemy) : base(enemy)
    {
        enemy1 = enemy as Enemy1;
    }

    private readonly Enemy1 enemy1;

    public override void Update()
    {
        base.Update();

        if (End_PlayerClose) {
            enemy1.ChangeState(enemy1.AttackState);
        }
        else if (End_PlayerVisible) {
            enemy1.ChangeState(enemy1.ChaseState);
        }
        else if (End_PlayerLost) {
            enemy1.ChangeState(enemy1.InvestigateAwareState);
        }
    }
}
