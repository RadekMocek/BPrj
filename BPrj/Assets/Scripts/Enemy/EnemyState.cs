using UnityEngine;

public class EnemyState
{
    protected Enemy enemy;

    protected float enterTime;

    public EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter()
    {
        Debug.Log($"{enemy.GetName()} changed state to {this.GetType().Name}.");

        enterTime = Time.time;
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Update()
    {

    }
}
