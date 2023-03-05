using UnityEngine;

public class Enemy1 : Enemy
{
    // == State machine =========================
    public Enemy1WanderState WanderState { get; private set; }

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
    private void Awake()
    {
        // States initialization
        WanderState = new Enemy1WanderState(this);
    }

    private void Start()
    {
        ChangeState(WanderState);
    }
}
