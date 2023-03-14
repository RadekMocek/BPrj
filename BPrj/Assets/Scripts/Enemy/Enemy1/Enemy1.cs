using UnityEngine;

public class Enemy1 : Enemy
{
    // == State machine =========================
    public Enemy1PatrolState PatrolState { get; private set; }
    public Enemy1DetectingState DetectingState { get; private set; }
    public Enemy1ChaseState ChaseState { get; private set; }
    public Enemy1InvestigateState InvestigateState { get; private set; }

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
        PatrolState = new Enemy1PatrolState(this);
        DetectingState = new Enemy1DetectingState(this);
        ChaseState = new Enemy1ChaseState(this);
        InvestigateState = new Enemy1InvestigateState(this);
    }

    protected override void Start()
    {
        base.Start();

        ChangeState(PatrolState);
    }

}
