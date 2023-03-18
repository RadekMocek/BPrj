using UnityEngine;

public class EnemyInvestigateSuspiciousState : EnemyInvestigateSuperState
{
    public EnemyInvestigateSuspiciousState(Enemy enemy) : base(enemy)
    {
    }

    public override void Update()
    {
        // Decrease red cone radius over time
        enemy.UpdateDecreaseViewConeRedRadius();

        // Super state logic
        base.Update();
    }
}
