using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(Enemy enemy) : base(enemy)
    {
    }

    /*
    int tempIndex;
    float tempTime;
    int tempCircles;

    public override void Enter()
    {
        base.Enter();

        tempIndex = 4;
        tempTime = Time.time;
        tempCircles = 0;
    }
    /**/

    public override void Update()
    {
        base.Update();

        enemy.RB.velocity = Vector2.zero;
        /*
        if (Time.time > tempTime + 1) {
            
            if (tempCircles >= 2) {
                tempIndex = (tempIndex + 1) % 8;
            }
            else {
                tempIndex--;
                if (tempIndex == -1) {
                    tempIndex = 7;
                    tempCircles++;
                }
            }
            
            tempTime = Time.time;
            enemy.ChangeFacingDiretion((EightDirection)tempIndex);
        }
        /**/
    }
}
