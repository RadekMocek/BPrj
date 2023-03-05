using UnityEngine;

public class Enemy : MonoBehaviour, IObservable, IDamageable
{
    // == Component references ==================
    public Rigidbody2D RB { get; private set; }

    // == State machine =========================
    private EnemyState currentState;
    
    public void ChangeState(EnemyState newState)
    {
        newState.Enter();
        currentState = newState;
    }

    // == Observe ===============================
    public virtual string GetName()
    {
        throw new System.NotImplementedException();
    }

    // == Receive damage ========================
    public virtual void ReceiveDamage(Vector2 direction)
    {
        throw new System.NotImplementedException();
    }

    // == MonoBehaviour functions ===============
    private void Awake()
    {
        // Component initialization
        RB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        // State logic
        currentState.FixedUpdate();
    }

    private void Update()
    { 
        // State logic
        currentState.Update();
    }
}
