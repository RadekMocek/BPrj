using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour, IObservable, IDamageable
{
    [field: Header("Transforms")]
    [field: SerializeField] public Transform Core { get; private set; }

    // == Component references ==================
    public Rigidbody2D RB { get; private set; }
    private Animator Anim { get; set; }

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
    public float TargetFacingDirection { get; set; }
    private float actualFacingDirection;
    public Direction CurrentFacingDirectionAnimation { get; private set; }

    private readonly int facingDirectionSpeed = 480;

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
                ChangeFacingDirection(Direction.SE);
            }
            else if (value.y > 0) { 
                ChangeFacingDirection(Direction.NE);
            }
            else { 
                ChangeFacingDirection(Direction.E);
            }
        }
        else if (value.x < 0) {
            if (value.y < 0) {
                ChangeFacingDirection(Direction.SW);
            }
            else if (value.y > 0) {
                ChangeFacingDirection(Direction.NW);
            }
            else {
                ChangeFacingDirection(Direction.W);
            }
        }
        else {
            if (value.y < 0) {
                ChangeFacingDirection(Direction.S);
            }
            else {
                ChangeFacingDirection(Direction.N);
            }
        }
    }

    public void DirectionToAnimation(Vector2 value)
    {
        if (value.x > 0) {
            if (value.y < 0) {
                Anim.CrossFade("EnemyDownRight", 0);
                CurrentFacingDirectionAnimation = Direction.SE;
            }
            else if (value.y > 0) {
                Anim.CrossFade("EnemyUpRight", 0);
                CurrentFacingDirectionAnimation = Direction.NE;
            }
            else {
                Anim.CrossFade("EnemyRight", 0);
                CurrentFacingDirectionAnimation = Direction.E;
            }
        }
        else if (value.x < 0) {
            if (value.y < 0) {
                Anim.CrossFade("EnemyDownLeft", 0);
                CurrentFacingDirectionAnimation = Direction.SW;
            }
            else if (value.y > 0) {
                Anim.CrossFade("EnemyUpLeft", 0);
                CurrentFacingDirectionAnimation = Direction.NW;
            }
            else {
                Anim.CrossFade("EnemyLeft", 0);
                CurrentFacingDirectionAnimation = Direction.W;
            }
        }
        else {
            if (value.y < 0) {
                Anim.CrossFade("EnemyDown", 0);
                CurrentFacingDirectionAnimation = Direction.S;
            }
            else {
                Anim.CrossFade("EnemyUp", 0);
                CurrentFacingDirectionAnimation = Direction.N;
            }
        }
    }

    public void MovementToAnimation() => DirectionToAnimation(DirectionRound(RB.velocity));

    public void ChangeFacingDirection(Direction direction) => TargetFacingDirection = (int)direction * 45;

    public Vector2 FacingDirectionToDirectionRound(float angle) => DirectionRound(new(Mathf.Cos(Mathf.Deg2Rad * (angle + 90)), Mathf.Sin(Mathf.Deg2Rad * (angle + 90))));

    private void UpdateActualFacingDirection()
    {
        float addition = Time.deltaTime * facingDirectionSpeed;

        if (Mathf.Abs(actualFacingDirection - TargetFacingDirection) <= addition) {
            return;
        }

        int directionMultiplier = (actualFacingDirection < TargetFacingDirection) ? 1 : -1;

        // Check if it is "cheaper" to cross the 0/360 point (rotate in opposite direciton)
        float angle1 = (actualFacingDirection < TargetFacingDirection) ? actualFacingDirection : TargetFacingDirection;
        float angle2 = (actualFacingDirection > TargetFacingDirection) ? actualFacingDirection : TargetFacingDirection;
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
    [SerializeField] private LayerMask playerLayer;

    private readonly int fieldOfView = 100;
    public readonly float viewDistance = 8;

    private Vector2 playerPosition;
    private Vector2 enemyToPlayerVector;
    private float enemyToPlayerAngle;

    [HideInInspector] public Vector2 lastKnownPlayerPosition;
    [HideInInspector] public bool suspiciousDetection; // When true, enemy will always transition to the InvestigateSuspicious (or Chase) state after Detecting state

    private bool CloserToZeroCounterClockwise(float angle) => (360 - angle > angle);

    public bool IsPlayerVisible(bool debug = false)
    {
        playerPosition = EnemyManager.GetPlayerPosition();

        enemyToPlayerVector = playerPosition - (Vector2)this.transform.position;

        // Is player in view distance ?
        if (enemyToPlayerVector.magnitude > viewDistance) {
            if (debug) print("Player too far away");
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
            if (debug) print("Player not in the FOV");
            return false;
        }

        // Is there a clear vision of the player ? (nothing obstructing the way)
        if (!Physics2D.Linecast(this.transform.position, playerPosition, opaqueLayer)) {
            // Are there any closed doors in the way ?
            Physics2D.queriesHitTriggers = false; // Ignore trigger colliders (door has big trigger collider for cursor)
            var doors = Physics2D.LinecastAll(this.transform.position, playerPosition, doorLayer);
            Physics2D.queriesHitTriggers = true; // Stop ignoring trigger colliders
            foreach (var door in doors) {
                if (door.transform.TryGetComponent(out Door doorScript)) {
                    if (!doorScript.Opened) {
                        if (debug) print("No clear vision of the player � closed door");
                        return false;
                    }
                }
            }
            return true;
        }
        else {
            if (debug) print("No clear vision of the player � opaqueLayer");
            return false;
        }
    }

    public void FaceThePlayer(bool changeAnimation)
    {
        TargetFacingDirection = enemyToPlayerAngle;
        if (changeAnimation) DirectionToAnimation(DirectionRound(enemyToPlayerVector));
    }

    public float GetEnemyToPlayerDistance() => enemyToPlayerVector.magnitude;

    // == Spotting the player: Close ============
    private readonly float playerCheckCloseRadius = 0.8f;
    private readonly Vector2 realBottom = new(0, 0.41f);

    private Vector2 playerCheckClose;

    public bool IsPlayerVisibleClose()
    {
        playerCheckClose = (Vector2)this.transform.position + realBottom + (playerCheckCloseRadius * FacingDirectionToDirectionRound(TargetFacingDirection));
        return Physics2D.OverlapCircle(playerCheckClose, playerCheckCloseRadius, playerLayer);
    }

    // == View Cone =============================
    [Header("View cone")]
    [SerializeField] private GameObject viewConeLightGO;
    private Light2D viewConeLightScript;

    [Header("View cone � Red")]
    [SerializeField] private GameObject viewConeRedLightGO;
    private Light2D viewConeRedLightScript;
    public float CurrentDetectionLength { get; private set; }

    private readonly int decreaseViewConeRedRadiusSpeed = 6;

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

    public void ChangeViewConeRedRadius(float value)
    {
        CurrentDetectionLength = value;
        viewConeRedLightScript.pointLightOuterRadius = CurrentDetectionLength;
    }

    public void UpdateDecreaseViewConeRedRadius()
    {
        if (CurrentDetectionLength == 0) return;

        CurrentDetectionLength -= Time.deltaTime * decreaseViewConeRedRadiusSpeed;

        if (CurrentDetectionLength < 0) CurrentDetectionLength = 0;

        viewConeRedLightScript.pointLightOuterRadius = CurrentDetectionLength;
    }

    // == Pathfinding ===========================
    public PathGrid Pathfinder { get; private set; }
    [Header("Pathfinding")]
    [SerializeField] private LayerMask unwalkableLayer;

    // == Patrol ================================
    [Header("Patrol state")]
    [SerializeField] private Vector2[] patrolPoints;
    public Vector2[] GetPatrolPoints() => patrolPoints;

    // == Weapon handling =======================
    [Header("Weapon")]
    [SerializeField] private GameObject weaponPrefab;
    private GameObject weaponGO;
    public Transform WeaponTransform { get; private set; }
    private BoxCollider2D weaponBC;
    public SpriteRenderer WeaponSR { get; private set; }

    public void StartWeapon()
    {
        weaponGO = Instantiate(weaponPrefab, this.transform);
        WeaponTransform = weaponGO.transform;
        weaponBC = weaponGO.GetComponent<BoxCollider2D>();
        weaponBC.enabled = false;
        WeaponSR = weaponGO.GetComponent<SpriteRenderer>();
        WeaponSR.sortingLayerName = "Player";
    }

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
        ChangeFacingDirection(Direction.S); // Facing down
        suspiciousDetection = false;

        // View cone
        StartViewCone();
        CurrentDetectionLength = 0;
        ChangeViewConeRedRadius(CurrentDetectionLength);

        // Weapon
        StartWeapon();
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
        // Open door
        if (collision.transform.TryGetComponent(out Door collisionDoorScript)) {
            collisionDoorScript.OpenDoor();
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
        /*
        Gizmos.DrawRay(this.transform.position, enemyToPlayerVector);
        Gizmos.DrawWireSphere((Vector2)this.transform.position + realBottom + (playerCheckCloseRadius * FacingDirectionToDirectionRound(TargetFacingDirection)), playerCheckCloseRadius);
        /**/
    }
}
