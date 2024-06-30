using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Melee, Ranged }
    public enum EnemyState { Idle, Patrolling, Chasing, Searching, Attacking }

    [Header("Characteristics")]
    public EnemyType enemyType;
    [Space]
    public float maxHealth;
    public float healthAmount;
    [Space]
    public float damage;
    public float attackRange;
    public float attackCooldown;
    private float lastAttackTime;
    [Space]
    public float movementSpeed;
    [Space]
    public float detectionRadius;

    [Header("Patrol")]
    public GameObject exclamationMarkPrefab;
    [Space]
    public float patrolSpeed;
    public float patrolDuration;
    public float patrolWaitTime;
    public float patrolRadius;
    [Space]
    public LayerMask obstacleLayer;
    private Vector2 patrolDirection;
    private float patrolTimer;
    private float patrolWaitTimer;
    private Vector2 distanceToPlayer;
    private Vector3 playerColliderOffset = new Vector3(0, -0.3f, 0);
    private Vector3 lastKnownPlayerPosition;
    private bool sawPlayer = false;

    [Header("States")]
    public Color idleColor = Color.white;
    public Color patrolColor = Color.green;
    public Color chasingColor = Color.yellow;
    public Color attackingColor = Color.red;
    public Color searchingColor = Color.blue;
    private EnemyState currentState = EnemyState.Idle;

    [Header("Components")]
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyManager enemyManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        enemyManager.addToList(gameObject);

        healthAmount = maxHealth;

        SetState(EnemyState.Idle);
    }

    void Update()
    {
        distanceToPlayer = player.position + playerColliderOffset - transform.position;

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
                    lastKnownPlayerPosition = player.position + playerColliderOffset;
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
                    SetState(EnemyState.Searching);
                }
                break;

            case EnemyState.Searching:
                if (CanSeePlayer())
                {
                    DetectPlayer();
                }
                else
                {
                    SearchForPlayer();
                }
                break;

            case EnemyState.Attacking:
                if (CanSeePlayer())
                {
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
                }
                else
                {
                    SetState(EnemyState.Searching);
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
            case EnemyState.Searching:
                spriteRenderer.color = searchingColor;
                break;
            case EnemyState.Attacking:
                spriteRenderer.color = attackingColor;
                break;
        }
    }

    bool CanSeePlayer()
    {
        if (distanceToPlayer.magnitude <= detectionRadius)
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position + playerColliderOffset, obstacleLayer);

            if (hit.collider == null)
            {
                return true;
            }
        }
        return false;
    }

    void DetectPlayer()
    {
        if(!sawPlayer){
            sawPlayer = true;
            var exclamationMark = Instantiate(exclamationMarkPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            exclamationMark.transform.SetParent(transform);
        }

        SetState(EnemyState.Chasing);
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position + playerColliderOffset - transform.position).normalized;
        rb.velocity = direction * movementSpeed;
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

    void SearchForPlayer()
    {
        Vector2 direction = (lastKnownPlayerPosition - transform.position).normalized;
        rb.velocity = direction * movementSpeed;

        if (Vector2.Distance(transform.position, lastKnownPlayerPosition) < 0.1f)
        {
            SetState(EnemyState.Patrolling);
        }
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
        //playerHealthManager.TakeDamage(damage);

        // Implement melee attack logic here
    }

    void RangedAttack()
    {
        Debug.Log("Ranged attack!");
        //playerHealthManager.TakeDamage(damage);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == EnemyState.Patrolling && ((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            SetNewPatrolDirection();
        }
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;

        if (healthAmount <= 0)
        {
            Die();
        }

    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);
    }

    void Die()
    {
        enemyManager.removeFromList(gameObject);
        Destroy(gameObject);
    }

    void UpdateVisuals()
    {
        animator.SetBool("Run", rb.velocity.magnitude != 0);

        if (rb.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        if(distanceToPlayer.magnitude < detectionRadius){
            Gizmos.DrawRay(transform.position, distanceToPlayer);
        }else{
            Gizmos.DrawRay(transform.position, distanceToPlayer.normalized * detectionRadius);
        }
        

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}
