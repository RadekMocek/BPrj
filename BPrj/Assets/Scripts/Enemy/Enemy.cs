using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour, IObservable, IDamageable, IObservableHealth
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

    // Observe health
    public (int, int) GetHealthInfo() => (health, maxHealth);

    // == Health, Receive damage, Knockback =====
    [Header("Receive damage")]
    [SerializeField] private GameObject hitParticlePrefab;
    [SerializeField] private GameObject deadParticlePrefab;

    private readonly int maxHealth = 100;
    private int health;
    private bool dead;
    public Vector2 KnockbackDirection { get; private set; }

    public virtual void ReceiveDamage(Vector2 direction, int amount)
    {
        health -= amount;
        
        if (health <= 0 && !dead) {
            // Die():
            Instantiate(deadParticlePrefab, this.Core.position, Quaternion.identity);
            CameraShake.Instance.ShakeCamera(6);

            RB.mass = 35;
            RB.drag = 35;

            dead = true;
        }
        else {
            Instantiate(hitParticlePrefab, this.Core.position, Quaternion.identity);
            CameraShake.Instance.ShakeCamera();
        }
        KnockbackDirection = direction;
    }

    // == Direction =============================
    public float TargetFacingDirection { get; set; }
    private float actualFacingDirection;
    public Direction CurrentFacingDirectionAnimation { get; private set; }

    private readonly int facingDirectionSpeed = 480;

    private Vector2 DirectionRound(Vector2 value, float treshold = 0.6f)
    {
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
            if (value.y > 0) {
                Anim.CrossFade("EnemyUp", 0);
                CurrentFacingDirectionAnimation = Direction.N;
            }
            else {
                Anim.CrossFade("EnemyDown", 0);
                CurrentFacingDirectionAnimation = Direction.S;
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

    private readonly int fieldOfView = 105;
    public readonly float viewDistance = 8.2f;

    private Vector2 playerPosition;
    public Vector2 EnemyToPlayerVector { get; private set; }
    public float EnemyToPlayerAngle { get; private set; }

    [HideInInspector] public Vector2 lastKnownPlayerPosition;
    [HideInInspector] public bool suspiciousDetection; // When true, enemy will always transition to the InvestigateSuspicious (or Chase) state after Detecting state

    public void UpdatePlayerPositionInfo()
    {
        playerPosition = EnemyManager.GetPlayerPosition();
        EnemyToPlayerVector = playerPosition - (Vector2)this.transform.position;
        EnemyToPlayerAngle = ((Mathf.Rad2Deg * Mathf.Atan2(EnemyToPlayerVector.y, EnemyToPlayerVector.x)) + 270) % 360;
    }

    public bool IsPlayerVisible(bool debug = false)
    {
        UpdatePlayerPositionInfo();

        // Is player in view distance ?
        if (EnemyToPlayerVector.magnitude > viewDistance) {
            if (debug) print("Player too far away");
            return false;
        }

        // Is player inside the field of view cone ?
        bool isPlayerInFieldOfView;
        
        int angleBound1 = (int)actualFacingDirection - fieldOfView / 2;
        int angleBound2 = (int)actualFacingDirection + fieldOfView / 2;

        if (angleBound1 < 0) angleBound1 += 360;
        if (angleBound2 > 360) angleBound2 -= 360;

        if (angleBound1 > angleBound2) {
            if (360 - EnemyToPlayerAngle > EnemyToPlayerAngle) { // If the player is in the 0–something part of the circle
                isPlayerInFieldOfView = (EnemyToPlayerAngle < angleBound2);
            }
            else {                                              // If the player is in the something–360 part of the circle
                isPlayerInFieldOfView = (EnemyToPlayerAngle > angleBound1);
            }
        }
        else {
            isPlayerInFieldOfView = (EnemyToPlayerAngle > angleBound1 && EnemyToPlayerAngle < angleBound2);
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
                        if (debug) print("No clear vision of the player – closed door");
                        return false;
                    }
                }
            }
            return true;
        }
        else {
            if (debug) print("No clear vision of the player – opaqueLayer");
            return false;
        }
    }

    public void FaceThePlayer(bool changeAnimation)
    {
        UpdatePlayerPositionInfo();
        TargetFacingDirection = EnemyToPlayerAngle;
        if (changeAnimation) DirectionToAnimation(DirectionRound(EnemyToPlayerVector, 0.25f));
    }

    public float GetEnemyToPlayerDistance() => EnemyToPlayerVector.magnitude;

    // == Spotting the player: Close ============
    private readonly float playerCheckCloseRadius = 0.7f;
    private readonly float playerCheckCloseDistance = 0.35f;
    private readonly Vector2 realBottom = new(0, 0.41f); // Center of gravity

    private Vector2 playerCheckClose;

    public bool IsPlayerVisibleClose()
    {
        playerCheckClose = (Vector2)this.transform.position + realBottom + (playerCheckCloseDistance * FacingDirectionToDirectionRound(TargetFacingDirection));
        return Physics2D.OverlapCircle(playerCheckClose, playerCheckCloseRadius, playerLayer);
    }

    // == View Cone =============================
    [Header("View cone")]
    [SerializeField] private GameObject viewConeLightGO;
    private Light2D viewConeLightScript;

    [Header("View cone – Red")]
    [SerializeField] private GameObject viewConeRedLightGO;
    private Light2D viewConeRedLightScript;
    public float CurrentDetectionLength { get; private set; }

    private readonly int decreaseViewConeRedRadiusSpeed = 3;

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
        Pathfinder = new PathGrid();
    }

    protected virtual void Start()
    {
        // Initialize
        ChangeFacingDirection(Direction.S); // Facing down
        suspiciousDetection = false;

        // Health
        health = maxHealth;
        dead = false;

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
        // Dead logic
        if (dead) {
            RB.velocity = Vector2.zero;
            viewConeLightGO.SetActive(false);
            UpdateDecreaseViewConeRedRadius();
            if (CurrentDetectionLength == 0) {
                this.enabled = false;
            }

            return;
        }

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

    [HideInInspector] public Vector2 gizmoCircleCenter;
    [HideInInspector] public float gizmoCircleRadius;
    private void OnDrawGizmos()
    {
        /*
        Gizmos.DrawWireSphere(gizmoCircleCenter, gizmoCircleRadius);
        //Gizmos.DrawRay(this.transform.position, EnemyToPlayerVector);
        Gizmos.DrawWireSphere((Vector2)this.transform.position + realBottom + (playerCheckCloseDistance * FacingDirectionToDirectionRound(TargetFacingDirection)), playerCheckCloseRadius);
        /**/
    }
}
