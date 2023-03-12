using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour, IObservable, IDamageable
{
    // == Component references ==================
    public Rigidbody2D RB { get; private set; }
    private Animator Anim { get; /*private*/ set; }

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

    public void MovementToFacingDirectionAndAnimation(Vector2 value)
    {
        float treshold = 0.5f;

        if (value.x < treshold && value.x > -treshold) {
            value.x = 0;
        }

        if (value.y < treshold && value.y > -treshold) {
            value.y = 0;
        }

        if (value.x > 0) {
            if (value.y < 0) { 
                ChangeFacingDiretion(EightDirection.SE);
                Anim.CrossFade("EnemyDown", 0);
            }
            else if (value.y > 0) { 
                ChangeFacingDiretion(EightDirection.NE);
                Anim.CrossFade("EnemyUp", 0);
            }
            else { 
                ChangeFacingDiretion(EightDirection.E);
                Anim.CrossFade("EnemyRight", 0);
            }
        }
        else if (value.x < 0) {
            if (value.y < 0) {
                ChangeFacingDiretion(EightDirection.SW);
                Anim.CrossFade("EnemyDown", 0);
            }
            else if (value.y > 0) {
                ChangeFacingDiretion(EightDirection.NW);
                Anim.CrossFade("EnemyUp", 0);
            }
            else {
                ChangeFacingDiretion(EightDirection.W);
                Anim.CrossFade("EnemyLeft", 0);
            }
        }
        else {
            if (value.y < 0) {
                ChangeFacingDiretion(EightDirection.S);
                Anim.CrossFade("EnemyDown", 0);
            }
            else {
                ChangeFacingDiretion(EightDirection.N);
                Anim.CrossFade("EnemyUp", 0);
            }
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

        // Check if it is "cheaper" to cross the 0/360 point (go opposite direciton)
        float angle1 = (actualFacingDirection < targetFacingDirection) ? actualFacingDirection : targetFacingDirection;
        float angle2 = (actualFacingDirection > targetFacingDirection) ? actualFacingDirection : targetFacingDirection;
        if ((angle1 + (360 - angle2)) < (angle2 - angle1)) directionMultiplier *= -1;

        actualFacingDirection += addition * directionMultiplier;

        if (actualFacingDirection > 360) actualFacingDirection -= 360;
        else if (actualFacingDirection < 0) actualFacingDirection += 360;
    }

    // == EnemyManager, spotting the player =====
    public EnemyManager EnemyManager { get; private set; }

    [Header("Spotting the player")]
    [SerializeField] private LayerMask opaqueLayer;
    [SerializeField] private LayerMask doorLayer;

    private readonly int fieldOfView = 90;
    private readonly float viewDistance = 6;

    private Vector2 playerPosition;
    private Vector2 enemyToPlayerVector;

    private bool CloserToZeroCounterClockwise(float angle) => (360 - angle > angle);

    private bool IsPlayerVisible()
    {
        playerPosition = EnemyManager.GetPlayerPosition();
        enemyToPlayerVector = playerPosition - (Vector2)this.transform.position;

        // Is player in view distance ?
        if (enemyToPlayerVector.magnitude > viewDistance) {
            return false;
        }

        // Is player inside the field of view cone ?
        bool isPlayerInFiledOfView;
        float angle = ((Mathf.Rad2Deg * Mathf.Atan2(enemyToPlayerVector.y, enemyToPlayerVector.x)) + 270) % 360;

        int angleBound1 = (int)actualFacingDirection - fieldOfView / 2;
        int angleBound2 = (int)actualFacingDirection + fieldOfView / 2;

        if (angleBound1 < 0) angleBound1 += 360;

        if (angleBound1 > angleBound2) {
            if (CloserToZeroCounterClockwise(angle)) {
                isPlayerInFiledOfView = (angle < angleBound2);
            }
            else {
                isPlayerInFiledOfView = (angle > angleBound1);
            }
        }
        else {
            isPlayerInFiledOfView = (angle > angleBound1 && angle < angleBound2);
        }

        if (!isPlayerInFiledOfView) {
            return false;
        }

        // Is there a clear vision of the player ? (nothing obstructing the way)
        if (!Physics2D.Linecast(this.transform.position, playerPosition, opaqueLayer)) {
            // Are there any closed doors in the way ?
            var doors = Physics2D.LinecastAll(this.transform.position, playerPosition, doorLayer);
            foreach (var door in doors) {
                if (door.transform.TryGetComponent(out Door doorScript)) {
                    if (!doorScript.Opened) {
                        return false;
                    }
                }
            }
            return true;
        }
        else return false;
    }

    // == View Cone =============================
    [Header("View cone")]
    [SerializeField] private GameObject viewConeLightGO;
    private Light2D viewConeLightScript;

    private void StartViewCone()
    {
        viewConeLightScript.pointLightOuterAngle = fieldOfView;
        viewConeLightScript.pointLightOuterRadius = viewDistance;
    }

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
        Anim = GetComponent<Animator>();
        viewConeLightScript = viewConeLightGO.GetComponent<Light2D>();

        // Services initialization
        EnemyManager = ManagerAccessor.instance.EnemyManager;
        Pathfinder = new PathGrid(unwalkableLayer);
    }

    protected virtual void Start()
    {
        // Initialize
        ChangeFacingDiretion(EightDirection.S); // Facing down
        StartViewCone();
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

        print(IsPlayerVisible());
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

    private void OnDrawGizmos()
    {
        //Gizmos.DrawRay(this.transform.position, enemyToPlayerVector);
    }
}
