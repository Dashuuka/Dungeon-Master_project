using UnityEngine;
using static HealthManager;
public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Melee, Ranged }
    public enum EnemyState { Idle, Patrolling, Chasing, Attacking }

    private HealthManager playerHealthManager;
    public EnemyType enemyType;
    public float detectionRadius;
    public float attackRange;
    public float attackCooldown;
    public float movementSpeed;
    public GameObject exclamationMarkPrefab;
    public float patrolSpeed;
    public float patrolDuration;
    public float patrolWaitTime;
    public float patrolRadius;
    public LayerMask obstacleLayer;

    private Transform player;
    private Rigidbody2D rb;
    private float lastAttackTime;
    private bool isPlayerDetected = false;
    private Vector2 distanceToPlayer;
    private Vector2 patrolDirection;
    private float patrolTimer;
    private float patrolWaitTimer;
    private EnemyState currentState = EnemyState.Idle;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 rayDirection;

    // Цвета для разных состояний
    private Color idleColor = Color.white;
    private Color patrolColor = Color.green;
    private Color chasingColor = Color.yellow;
    private Color attackingColor = Color.red;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        SetState(EnemyState.Idle);
    }

    void Update()
    {
        distanceToPlayer = player.position - transform.position;

        switch (currentState)
        {
            case EnemyState.Idle:
                if (CanSeePlayer())
                {
                    DetectPlayer();
                }
                else if (patrolWaitTimer <= 0)
                {
                    SetNewPatrolDirection();
                    SetState(EnemyState.Patrolling);
                }
                else
                {
                    patrolWaitTimer -= Time.deltaTime;
                }
                break;

            case EnemyState.Patrolling:
                if (CanSeePlayer())
                {
                    DetectPlayer();
                }
                else
                {
                    Patrol();
                }
                break;

            case EnemyState.Chasing:
                if (CanSeePlayer())
                {
                    if (distanceToPlayer.magnitude <= attackRange)
                    {
                        SetState(EnemyState.Attacking);
                    }
                    else
                    {
                        ChasePlayer();
                    }
                }
                else
                {
                    LosePlayer();
                }
                break;

            case EnemyState.Attacking:
            if (CanSeePlayer()){
                    if (distanceToPlayer.magnitude <= attackRange)
                    {
                        if (Time.time - lastAttackTime >= attackCooldown)
                        {
                            Attack();
                        }
                    }
                    else
                    {
                        SetState(EnemyState.Chasing);
                    }
                }else{
                    LosePlayer();
                }
            break;
        }

        UpdateVisuals();
    }

    void SetState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            UpdateColor();
            Debug.Log($"Enemy state changed to: {currentState}");
        }
    }

    void UpdateColor()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                spriteRenderer.color = idleColor;
                break;
            case EnemyState.Patrolling:
                spriteRenderer.color = patrolColor;
                break;
            case EnemyState.Chasing:
                spriteRenderer.color = chasingColor;
                break;
            case EnemyState.Attacking:
                spriteRenderer.color = attackingColor;
                break;
        }
    }

    bool CanSeePlayer(){
    if (distanceToPlayer.magnitude <= detectionRadius)
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleLayer);
        if (hit.collider == null)
        {
            return true;
        }
    }
    return false;
}


    void DetectPlayer()
    {
        if (!isPlayerDetected)
        {
            isPlayerDetected = true;
            var exclamationMark = Instantiate(exclamationMarkPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            exclamationMark.transform.SetParent(transform);
        }
        SetState(EnemyState.Chasing);
    }

    void LosePlayer()
    {
        isPlayerDetected = false;
        SetState(EnemyState.Idle);
        rb.velocity = Vector2.zero;
        patrolWaitTimer = patrolWaitTime;
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * movementSpeed;
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        if (enemyType == EnemyType.Melee)
        {
            MeleeAttack();
        }
        else
        {
            RangedAttack();
        }
    }

    void MeleeAttack()
    {
        Debug.Log("Melee attack!");
        playerHealthManager=player.GetComponent<HealthManager>();
        playerHealthManager.TakeDamage(10);

        // Implement melee attack logic here
    }

    void RangedAttack()
    {
        Debug.Log("Ranged attack!");
        playerHealthManager = player.GetComponent<HealthManager>();
        playerHealthManager.TakeDamage(20);
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0)
        {
            SetState(EnemyState.Idle);
            rb.velocity = Vector2.zero;
            patrolWaitTimer = patrolWaitTime;
        }
        else
        {
            rb.velocity = patrolDirection * patrolSpeed;
        }
    }

    void SetNewPatrolDirection()
    {
        patrolDirection = Random.insideUnitCircle.normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, patrolDirection, patrolRadius, obstacleLayer);
        
        if (hit.collider != null)
        {
            patrolDirection = Vector2.Reflect(patrolDirection, hit.normal);
        }
        
        patrolTimer = patrolDuration;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == EnemyState.Patrolling && ((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            SetNewPatrolDirection();
        }
    }

    void UpdateVisuals(){
        animator.SetBool("Run", rb.velocity.magnitude != 0);

        if (rb.velocity.x > 0)
        {
            transform.localScale = Vector3.one;
        }
        else if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (enemyType == EnemyType.Ranged)
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(transform.position, rangedPreferredDistance);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, distanceToPlayer.normalized * detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}