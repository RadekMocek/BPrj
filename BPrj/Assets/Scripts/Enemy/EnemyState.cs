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
        //Debug.Log($"{enemy.GetName()} changed state to {this.GetType().Name}.");

        enterTime = Time.time;
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Update()
    {

    }

    // Update equipped weapon position, rotation, order in layer
    public virtual void UpdateWeaponPosition()
    {
        var pos = Weapon.GetCorrectWeaponPosition(enemy.CurrentFacingDirectionAnimation);
        enemy.WeaponSR.sortingOrder = pos.Item3;
        enemy.WeaponTransform.SetLocalPositionAndRotation(pos.Item1, pos.Item2);
    }
}
