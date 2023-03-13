using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class Enemy : MonoBehaviour, IObservable, IDamageable
{
    [field: Header("Transforms")]
    [field: SerializeField] public Transform Core { get; private set; }

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

    private Vector2 DirectionRound(Vector2 value)
    {
        float treshold = 0.5f;

        if (value.x < treshold && value.x > -treshold) {
            value.x = 0;
        }

        if (value.y < treshold && value.y > -treshold) {
            value.y = 0;
        }

        return value;
    }

    public void DirectionToFacingDirectionAndAnimation(Vector2 value)
    {
        value = DirectionRound(value);

        DirectionToAnimation(value);

        if (value.x > 0) {
            if (value.y < 0) { 
                ChangeFacingDirection(EightDirection.SE);
            }
            else if (value.y > 0) { 
                ChangeFacingDirection(EightDirection.NE);
            }
            else { 
                ChangeFacingDirection(EightDirection.E);
            }
        }
        else if (value.x < 0) {
            if (value.y < 0) {
                ChangeFacingDirection(EightDirection.SW);
            }
            else if (value.y > 0) {
                ChangeFacingDirection(EightDirection.NW);
            }
            else {
                ChangeFacingDirection(EightDirection.W);
            }
        }
        else {
            if (value.y < 0) {
                ChangeFacingDirection(EightDirection.S);
            }
            else {
                ChangeFacingDirection(EightDirection.N);
            }
        }
    }

    private void DirectionToAnimation(Vector2 value)
    {
        if (value.x > 0) {
            if (value.y < 0) {
                Anim.CrossFade("EnemyDownRight", 0);
            }
            else if (value.y > 0) {
                Anim.CrossFade("EnemyUpRight", 0);
            }
            else {
                Anim.CrossFade("EnemyRight", 0);
            }
        }
        else if (value.x < 0) {
            if (value.y < 0) {
                Anim.CrossFade("EnemyDownLeft", 0);
            }
            else if (value.y > 0) {
                Anim.CrossFade("EnemyUpLeft", 0);
            }
            else {
                Anim.CrossFade("EnemyLeft", 0);
            }
        }
        else {
            if (value.y < 0) {
                Anim.CrossFade("EnemyDown", 0);
            }
            else {
                Anim.CrossFade("EnemyUp", 0);
            }
        }
    }

    public void ChangeFacingDirection(EightDirection direction)
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

    private readonly int fieldOfView = 100;
    public readonly float viewDistance = 8;

    private Vector2 playerPosition;
    //private Vector2 playerCorePosition;
    private Vector2 enemyToPlayerVector;
    private float enemyToPlayerAngle;

    private bool CloserToZeroCounterClockwise(float angle) => (360 - angle > angle);

    public bool IsPlayerVisible(/*bool debug = false*/)
    {
        //playerCorePosition = EnemyManager.GetPlayerCorePosition();
        playerPosition = EnemyManager.GetPlayerPosition();

        enemyToPlayerVector = playerPosition - (Vector2)this.transform.position;

        // Is player in view distance ?
        if (enemyToPlayerVector.magnitude > viewDistance) {
            return false;
        }

        // Is player inside the field of view cone ?
        bool isPlayerInFieldOfView;
        enemyToPlayerAngle = ((Mathf.Rad2Deg * Mathf.Atan2(enemyToPlayerVector.y, enemyToPlayerVector.x)) + 270) % 360;
        
        int angleBound1 = (int)actualFacingDirection - fieldOfView / 2;
        int angleBound2 = (int)actualFacingDirection + fieldOfView / 2;

        if (angleBound1 < 0) angleBound1 += 360;
        if (angleBound2 > 360) angleBound2 -= 360;

        if (angleBound1 > angleBound2) {
            if (CloserToZeroCounterClockwise(enemyToPlayerAngle)) {
                isPlayerInFieldOfView = (enemyToPlayerAngle < angleBound2);
            }
            else {
                isPlayerInFieldOfView = (enemyToPlayerAngle > angleBound1);
            }
        }
        else {
            isPlayerInFieldOfView = (enemyToPlayerAngle > angleBound1 && enemyToPlayerAngle < angleBound2);
        }

        if (!isPlayerInFieldOfView) {
            return false;
        }

        // Is there a clear vision of the player ? (nothing obstructing the way)
        if (!Physics2D.Linecast(/*this.transform.position*/Core.position, playerPosition, opaqueLayer)) {
            // Are there any closed doors in the way ?
            var doors = Physics2D.LinecastAll(/*this.transform.position*/Core.position, playerPosition, doorLayer);
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

    public void FaceThePlayer()
    {
        targetFacingDirection = enemyToPlayerAngle;
        DirectionToAnimation(DirectionRound(enemyToPlayerVector));
    }

    public float GetEnemyToPlayerDistance() => enemyToPlayerVector.magnitude;

    // == View Cone =============================
    [Header("View cone")]
    [SerializeField] private GameObject viewConeLightGO;
    private Light2D viewConeLightScript;

    [Header("View cone – Red")]
    [SerializeField] private GameObject viewConeRedLightGO;
    private Light2D viewConeRedLightScript;

    private void StartViewCone()
    {
        viewConeLightScript.pointLightOuterAngle = fieldOfView;
        viewConeLightScript.pointLightOuterRadius = viewDistance;
        ChangeViewConeColor(Color.green);

        viewConeRedLightScript.pointLightOuterAngle = fieldOfView;
    }

    private void UpdateViewCone()
    {
        viewConeLightGO.transform.rotation = Quaternion.Euler(0, 0, actualFacingDirection);

        viewConeRedLightGO.transform.rotation = Quaternion.Euler(0, 0, actualFacingDirection);
    }

    public void ChangeViewConeColor(Color color)
    {
        viewConeLightScript.color = color;
    }

    public void ShowViewConeRed(bool value) => viewConeRedLightGO.SetActive(value);
    public void ChangeViewConeRedRadius(float value) => viewConeRedLightScript.pointLightOuterRadius = value;

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
        // - View cone
        viewConeLightScript = viewConeLightGO.GetComponent<Light2D>();
        viewConeRedLightScript = viewConeRedLightGO.GetComponent<Light2D>();

        // Services initialization
        EnemyManager = ManagerAccessor.instance.EnemyManager;
        Pathfinder = new PathGrid(unwalkableLayer);
    }

    protected virtual void Start()
    {
        // Initialize
        ChangeFacingDirection(EightDirection.S); // Facing down
        StartViewCone();
        viewConeRedLightGO.SetActive(false);
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
        Gizmos.DrawRay(this.transform.position, enemyToPlayerVector);
    }
}
