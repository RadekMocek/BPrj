using UnityEngine;

public class EnemyState
{
    protected Enemy enemy;

    public EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log($"{enemy.GetName()} changed state to {this.GetType().Name}.");
    }

    public void FixedUpdate()
    {

    }

    public void Update()
    {

    }
}
