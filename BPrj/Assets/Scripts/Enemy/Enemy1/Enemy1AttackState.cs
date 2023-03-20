using UnityEngine;

public class Enemy1AttackState : EnemyAttackState
{
    public Enemy1AttackState(Enemy enemy) : base(enemy)
    {
        enemy1 = enemy as Enemy1;
    }

    private readonly Enemy1 enemy1;
}
