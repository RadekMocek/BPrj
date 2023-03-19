using UnityEngine;

public class Enemy1 : Enemy
{
    // == State machine =========================
    public Enemy1PatrolState PatrolState { get; private set; }
    public Enemy1DetectingState DetectingState { get; private set; }
    public Enemy1ChaseState ChaseState { get; private set; }
    public Enemy1InvestigateSuspiciousState InvestigateSuspiciousState { get; private set; }
    public Enemy1InvestigateAwareState InvestigateAwareState { get; private set; }
    public Enemy1LookAroundState LookAroundState { get; private set; }

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
        InvestigateSuspiciousState = new Enemy1InvestigateSuspiciousState(this);
        InvestigateAwareState = new Enemy1InvestigateAwareState(this);
        LookAroundState = new Enemy1LookAroundState(this);
    }

    protected override void Start()
    {
        base.Start();

        //ChangeState(PatrolState);
        ChangeState(LookAroundState);
    }

}
