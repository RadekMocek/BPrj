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

    // == EnemyManager ==========================
    public EnemyManager EnemyManager { get; private set; }

    private void PlayerDirection()
    {
        Debug.Log(EnemyManager.GetPlayerPosition() - (Vector2)this.transform.position);
    }

    // == Pathfinding ===========================
    public PathGrid Pathfinder { get; private set; }
    [Header("Pathfinding")]
    [SerializeField] private LayerMask unwalkableLayer;

    // == Patrol ================================
    [Header("Patrol state")]
    [SerializeField] private Vector2[] patrolPoints;
    public Vector2[] GetPatrolPoints() => patrolPoints;

    // == MonoBehaviour functions ===============
    protected virtual void Awake()
    {
        // Component initialization
        RB = GetComponent<Rigidbody2D>();

        // Services initialization
        EnemyManager = ManagerAccessor.instance.EnemyManager;
        Pathfinder = new PathGrid(unwalkableLayer);
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        // State logic
        currentState.FixedUpdate();
    }

    protected virtual void Update()
    { 
        // State logic
        currentState.Update();

        // Player spotting
        PlayerDirection();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out Door collisionDoorSciprt)) {
            collisionDoorSciprt.OpenDoor();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolPoints != null) {
            foreach (var patrolPoint in patrolPoints) {
                Gizmos.DrawWireSphere(patrolPoint, .2f);
            }
        }
    }
}
