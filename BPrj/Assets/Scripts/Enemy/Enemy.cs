using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour, IObservable, IDamageable, IObservableHealth
{
    // == Scriptable object data ================
    [field: Header("Data")]
    [field: SerializeField] public EnemyDataSO Data { get; private set; }

    // == Transform references ==================
    [field: Header("Transforms")]
    [field: SerializeField] public Transform Core { get; private set; }

    // == Component references ==================
    private SpriteRenderer SR { get; set; }
    public Rigidbody2D RB { get; private set; }
    private Animator Anim { get; set; }

    // == State machine =========================
    protected EnemyState currentState;
    
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
    [SerializeField] private Sprite[] deadSprites;

    private int maxHealth; // SO (value is set in the ScriptableObject (in the editor))
    private int health;
    public bool IsDead { get; private set; }
    
    public Vector2 KnockbackDirection { get; private set; }

    public virtual void ReceiveDamage(Vector2 direction, int amount)
    {
        health -= amount;
        
        if (health <= 0 && !IsDead) {
            Die();
        }
        else {
            Instantiate(hitParticlePrefab, this.Core.position, Quaternion.identity);
            CameraShake.Instance.ShakeCamera();
        }
        KnockbackDirection = direction;
    }

    private void Die()
    {
        // Change sprite
        Anim.enabled = false;
        SR.sprite = deadSprites[(int)CurrentFacingDirectionAnimation];
        // Particles + camera shake
        Instantiate(deadParticlePrefab, this.Core.position, Quaternion.identity);
        CameraShake.Instance.ShakeCamera(10);
        // Add some weight to the object
        RB.mass = 35;
        RB.drag = 35;
        // What to do with the weapon ?
        /* Drop weapon
        Vector3 dropDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        WeaponSR.sortingLayerName = "Floor_Items";
        weaponBC.enabled = true;
        WeaponTransform.SetParent(null);
        WeaponTransform.SetPositionAndRotation(this.transform.position + dropDirection, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        weaponScript.equipped = false;
        /**/
        ///* Destroy weapon
        Destroy(weaponGO);
        /**/
        // Make unobservable
        this.gameObject.layer = LayerMask.NameToLayer("Enemy_Dead");
        // Make dead consistent
        ManagerAccessor.instance.ConsistencyManager.SetRecord(this.transform.name, false);
        // Decrease red view cone over time and then disable this script
        IsDead = true;
    }

    // == Direction =============================
    public float TargetFacingDirection { get; set; }
    private float actualFacingDirection;
    public Direction CurrentFacingDirectionAnimation { get; private set; }

    private readonly int facingDirectionSpeed = 480;

    private Vector2 DirectionRound(Vector2 value, float threshold = 0.6f)
    {
        if (value.x < threshold && value.x > -threshold) {
            value.x = 0;
        }

        if (value.y < threshold && value.y > -threshold) {
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

        // Check if it is "cheaper" to cross the 0/360 point (rotate in opposite direction)
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
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private LayerMask playerLayer;

    private int fieldOfView; // SO
    public float ViewDistance { get; private set; } // SO

    private Vector2 playerPosition;
    public Vector2 EnemyToPlayerVector { get; private set; }
    public float EnemyToPlayerAngle { get; private set; }
    public bool IsPlayerVisible { get; private set; }
    public bool IsPlayerVisibleOpaqueOnly { get; private set; }

    [HideInInspector] public Vector2 lastKnownPlayerPosition;
    [HideInInspector] public bool suspiciousDetection; // When true, enemy will always transition to the InvestigateSuspicious (or Chase) state after Detecting state

    public void UpdatePlayerPositionInfo()
    {
        playerPosition = EnemyManager.GetPlayerPosition();
        EnemyToPlayerVector = playerPosition - (Vector2)this.transform.position;
        EnemyToPlayerAngle = ((Mathf.Rad2Deg * Mathf.Atan2(EnemyToPlayerVector.y, EnemyToPlayerVector.x)) + 270) % 360;
    }

    private bool UpdateIsPlayerVisible(bool debug = false)
    {
        if (EnemyManager.isPlayerDead) return false;

        UpdatePlayerPositionInfo();

        // Is player in view distance ?
        if (EnemyToPlayerVector.magnitude > ViewDistance) {
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
        Physics2D.queriesHitTriggers = false;
        IsPlayerVisibleOpaqueOnly = !Physics2D.Linecast(this.transform.position, playerPosition, opaqueLayer);
        Physics2D.queriesHitTriggers = true;
        if (IsPlayerVisibleOpaqueOnly) {
            // Ignore trigger colliders
            Physics2D.queriesHitTriggers = false;
            // Are there any closed doors in the way ?
            var doors = Physics2D.LinecastAll(this.transform.position, playerPosition, doorLayer);
            // Are there any objects in the way ?
            var objectHit = Physics2D.Linecast(this.transform.position, playerPosition, objectLayer);
            // Stop ignoring trigger colliders
            Physics2D.queriesHitTriggers = true;
            // Objects check
            if (objectHit && EnemyManager.IsPlayerSneaking()) {
                if (debug) print("Player sneaking behind object");
                return false;
            }
            // Door check
            foreach (var door in doors) {
                if (door.transform.TryGetComponent(out Door doorScript)) {
                    if (!doorScript.IsOpened) {
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
        if (EnemyManager.isPlayerDead) return false;

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
        viewConeLightScript.pointLightOuterRadius = ViewDistance;
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

    public void UpdateDecreaseViewConeRedRadius(int speedMultiplier = 1)
    {
        if (CurrentDetectionLength == 0) return;

        CurrentDetectionLength -= Time.deltaTime * decreaseViewConeRedRadiusSpeed * speedMultiplier;

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
    private Weapon weaponScript;

    public void StartWeapon()
    {
        weaponGO = Instantiate(weaponPrefab, this.transform);

        WeaponTransform = weaponGO.transform;

        weaponBC = weaponGO.GetComponent<BoxCollider2D>();
        weaponBC.enabled = false;

        WeaponSR = weaponGO.GetComponent<SpriteRenderer>();

        weaponScript = weaponGO.GetComponent<Weapon>();
        weaponScript.equipped = true;

        WeaponSR.sortingLayerName = "Player";
    }

    // == MonoBehaviour functions ===============
    protected virtual void Awake()
    {
        // Component initialization
        SR = GetComponent<SpriteRenderer>();
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
        // Get data from ScriptableObject
        maxHealth = Data.MaxHealth;
        fieldOfView = Data.FieldOfView;
        ViewDistance = Data.ViewDistance;

        // Initialize
        ChangeFacingDirection(Direction.S); // Facing down
        suspiciousDetection = false;

        // Health
        health = maxHealth;
        IsDead = false;

        // View cone
        StartViewCone();
        CurrentDetectionLength = 0;
        ChangeViewConeRedRadius(CurrentDetectionLength);
        // - Final cutscene special case
        if (ManagerAccessor.instance.SceneManager.GetCurrentSceneName() == "Outside_End") ChangeViewConeRedRadius(ViewDistance);

        // Weapon
        StartWeapon();
    }

    private void FixedUpdate()
    {
        // State logic
        currentState.FixedUpdate();

        // Sub-functions
        IsPlayerVisible = UpdateIsPlayerVisible();
    }

    private void Update()
    {
        // Stop all logic if player in DialogueState (Except final cutscene)
        if (EnemyManager.IsPlayerInDialogueState() && ManagerAccessor.instance.SceneManager.GetCurrentSceneName() != "Outside_End") {
            RB.velocity = Vector2.zero;
            return;
        }

        // Dead logic
        if (IsDead) {
            RB.velocity = Vector2.zero;
            UpdateDecreaseViewConeRedRadius(2);
            viewConeLightScript.pointLightOuterRadius = viewConeRedLightScript.pointLightOuterRadius;
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

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Open door
        if (collision.transform.TryGetComponent(out Door collisionDoorScript)) {
            collisionDoorScript.OpenDoor();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (patrolPoints != null) {
            foreach (Vector2 patrolPoint in patrolPoints) {
                Gizmos.DrawWireSphere(patrolPoint, .2f);
            }
        }
    }
}
