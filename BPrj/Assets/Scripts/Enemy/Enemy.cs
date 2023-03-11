using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    // == Direction =============================
    private float targetFacingDirection;
    private float actualFacingDirection;

    public void ChangeFacingDiretion(Vector2 value)
    {
        float treshold = 0.5f;

        if (value.x < treshold && value.x > -treshold) {
            value.x = 0;
        }

        if (value.y < treshold && value.y > -treshold) {
            value.y = 0;
        }

        if (value.x > 0) {
            if (value.y < 0) ChangeFacingDiretion(EightDirection.SE);
            else if (value.y > 0) ChangeFacingDiretion(EightDirection.NE);
            else ChangeFacingDiretion(EightDirection.E);
        }
        else if (value.x < 0) {
            if (value.y < 0) ChangeFacingDiretion(EightDirection.SW);
            else if (value.y > 0) ChangeFacingDiretion(EightDirection.NW);
            else ChangeFacingDiretion(EightDirection.W);
        }
        else {
            if (value.y < 0) ChangeFacingDiretion(EightDirection.S);
            else ChangeFacingDiretion(EightDirection.N);
        }
    }

    public void ChangeFacingDiretion(EightDirection direction)
    {
        targetFacingDirection = (int)direction * 45;
    }

    private void UpdateActualFacingDirection()
    {
        float addition = Time.deltaTime * 480;

        if (Mathf.Abs(actualFacingDirection - targetFacingDirection) <= addition) return;

        int directionMultiplier = (actualFacingDirection < targetFacingDirection) ? 1 : -1;

        actualFacingDirection += addition * directionMultiplier;
    }

    // == EnemyManager ==========================
    public EnemyManager EnemyManager { get; private set; }

    private void PlayerDirection()
    {
        //Debug.Log(EnemyManager.GetPlayerPosition() - (Vector2)this.transform.position);
    }

    // == View Cone =============================
    [Header("View cone")]
    [SerializeField] private GameObject viewConeLightGO;
    private Light2D viewConeLightScript;

    private void UpdateViewCone()
    {
        viewConeLightGO.transform.rotation = Quaternion.Euler(0, 0, actualFacingDirection);
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
        viewConeLightScript = viewConeLightGO.GetComponent<Light2D>();

        // Services initialization
        EnemyManager = ManagerAccessor.instance.EnemyManager;
        Pathfinder = new PathGrid(unwalkableLayer);
    }

    protected virtual void Start()
    {
        // Initialize
        ChangeFacingDiretion(EightDirection.S); // Facing down
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

        // Update sub-functions
        UpdateActualFacingDirection();
        UpdateViewCone();

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
