using UnityEngine;

public class Enemy1 : Enemy
{
    // == State machine =========================
    public Enemy1IdleState IdleState { get; private set; }
    public Enemy1PatrolState PatrolState { get; private set; }

    // == Observe ===============================
    public override string GetName()
    {
        return "Enemy1";
    }

    // == Receive damage ========================
    public override void ReceiveDamage(Vector2 direction)
    {
        Debug.Log("Received damage");
    }

    // == MonoBehaviour functions ===============
    protected override void Awake()
    {
        base.Awake();

        // States initialization
        IdleState = new Enemy1IdleState(this);
        PatrolState = new Enemy1PatrolState(this);
    }

    protected override void Start()
    {
        base.Start();

        //ChangeState(IdleState);
        ChangeState(PatrolState);
    }

}
