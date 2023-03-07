using UnityEngine;

public class EnemyState
{
    protected Enemy enemy;

    public EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter()
    {
        Debug.Log($"{enemy.GetName()} changed state to {this.GetType().Name}.");
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Update()
    {

    }
}
