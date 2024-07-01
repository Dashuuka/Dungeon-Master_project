using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Melee, Ranged }
    public enum EnemyState { Idle, Patrolling, Chasing, Searching, Attacking }

    [Header("Enemy Settings")]
    public EnemyType enemyType;
    [Space]
    public float maxHP;
    public float currentHP;
    [Space]
    public float damage;
    public float attackRange;
    public float attackCooldown;
    [Space]
    public float movementSpeed;
    [Space]
    public float detectionRadius;

    [Header("Ranged")]
    public GunBehaviour gun;

    [Header("Patrol")]
    public float patrolSpeed;
    public float patrolDuration;
    private float patrolTimer;
    public float patrolWaitTime;
    private float patrolWaitTimer;
    public float patrolRadius;
    [Space]
    public LayerMask obstacleLayer;

    [Header("Searching")]
    public float maxSearchingTime;
    private float searchingTimer;

    [Header("States")]
    public Color idleColor = Color.white;
    public Color patrolColor = Color.green;
    public Color chasingColor = Color.yellow;
    public Color attackingColor = Color.red;
    public Color searchingColor = Color.blue;
    private EnemyState currentState = EnemyState.Idle;

    [Header("Marks")]
    public GameObject markPrefab;

    [Header("Components")]
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    private EnemyManager enemyManager;

    [Header("Private variables")]
    private Vector2 patrolDirection;
    private Vector2 distanceToPlayer;
    private Vector3 playerColliderOffset = new Vector3(0, -0.3f, 0);
    private Vector3 lastKnownPlayerPosition;
    private bool sawPlayer = false;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        enemyManager = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();
        enemyManager.addToList(gameObject);

        currentHP = maxHP;

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
                    searchingTimer = maxSearchingTime;
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
            CreateMark(0);
        }

        SetState(EnemyState.Chasing);
    }

    void CreateMark(int id){
        var mark = Instantiate(markPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        mark.transform.SetParent(transform);
        mark.GetComponent<MarkScript>().SetMark(id);
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position + playerColliderOffset - transform.position).normalized;
        rb.velocity = direction * movementSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        gun.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
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
        patrolDirection = UnityEngine.Random.insideUnitCircle.normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, patrolDirection, patrolRadius, obstacleLayer);
        
        if (hit.collider != null)
        {
            patrolDirection = Vector2.Reflect(patrolDirection, hit.normal);
        }
        
        patrolTimer = patrolDuration;
    }

    void SearchForPlayer()
    {

        searchingTimer -= Time.deltaTime;

        Vector2 direction = (lastKnownPlayerPosition - transform.position).normalized;
        rb.velocity = direction * movementSpeed;

        if (Vector2.Distance(transform.position, lastKnownPlayerPosition) < 0.1f || searchingTimer <= 0f)
        {
            sawPlayer = false;
            SetState(EnemyState.Patrolling);
            CreateMark(1);
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
        gun.Shoot(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == EnemyState.Patrolling && ((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            SetNewPatrolDirection();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            Die();
        }

    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
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
