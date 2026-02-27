using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("Base Stats")]
    public float maxHealth = 50f;
    public float damage = 10f;
    private float _currentHealth;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 360f;

    [Header("Combat Settings")]
    public bool isRanged = false; // Uncheck for Warrior/Knight, Check for Wizard
    public float attackRange = 2f; // Replaces 'stoppingDistance'
    public float attackCooldown = 1.5f; // Replaces 'fireRate'

    [Header("Ranged Only (Wizard)")]
    public GameObject projectilePrefab; // Your arrow/fireball prefab
    public Transform firePoint;

    private Transform _player;
    private Rigidbody _rb;
    private Animator _animator;
    private NavMeshAgent _agent;
    private float _attackTimer;

    void Start()
    {
        _currentHealth = maxHealth;
        _rb = GetComponent<Rigidbody>();

        // Get the animator from the child model
        _animator = GetComponentInChildren<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Safely grab the NavMeshAgent if the enemy has one
        _agent = GetComponent<NavMeshAgent>();
        if (_agent != null)
        {
            // This stops the AI from rotating the model, allowing your custom aiming to work!
            _agent.updateRotation = false;
        }
    }

    void Update()
    {
        if (_player == null) return;

        // 1. Attack Cooldown Logic
        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;

        // 2. Automate Attack Trigger
        float distance = Vector3.Distance(transform.position, _player.position);
        if (_attackTimer <= 0 && distance <= attackRange)
        {
            TriggerAttack();
            _attackTimer = attackCooldown;
        }
    }

    void FixedUpdate()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.position);

        // 1. FREEZE IF ATTACKING
        if (_animator != null)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack1"))
            {
                if (_agent != null) _agent.velocity = Vector3.zero;
                _rb.linearVelocity = Vector3.zero;
                return;
            }
        }

        // 2. SMART PATHFINDING (If they have a NavMeshAgent)
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.SetDestination(_player.position);

            if (distance > attackRange)
            {
                _agent.isStopped = false;
                _agent.speed = moveSpeed;

                // Aim exactly where the path is turning!
                Vector3 direction = (_agent.steeringTarget - transform.position).normalized;
                RotateTowards(new Vector2(direction.x, direction.z));

                if (_animator != null) _animator.SetFloat("Speed", _agent.velocity.magnitude);
            }
            else
            {
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;

                // Aim directly at the player to swing
                Vector3 direction = (_player.position - transform.position).normalized;
                RotateTowards(new Vector2(direction.x, direction.z));

                if (_animator != null) _animator.SetFloat("Speed", 0f);
            }
        }
        // 3. FALLBACK FOR DUMB ENEMIES (Like the Capsule)
        else
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            RotateTowards(new Vector2(direction.x, direction.z));

            if (distance > attackRange)
            {
                Vector3 velocity = new Vector3(direction.x, 0, direction.z) * moveSpeed;
                _rb.linearVelocity = velocity;

                if (_animator != null) _animator.SetFloat("Speed", velocity.magnitude);
            }
            else
            {
                _rb.linearVelocity = Vector3.zero;
                if (_animator != null) _animator.SetFloat("Speed", 0f);
            }
        }
    }

    void RotateTowards(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, angle, 0), rotationSpeed * Time.fixedDeltaTime);
    }

    void TriggerAttack()
    {
        // Tell the Animator to swing/cast
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }
        else
        {
            // FALLBACK: If this enemy has no animator (like the capsule), attack instantly!
            PerformAttack();
        }
    }

    // ANIMATION EVENT: Called exactly when the weapon swings or staff fires
    public void PerformAttack()
    {
        if (isRanged && projectilePrefab != null && firePoint != null)
        {
            // WIZARD: Shoot Projectile
            Instantiate(projectilePrefab, firePoint.position, transform.rotation);
        }
        else
        {
            // KNIGHT / WARRIOR: Melee Hit
            if (_player == null) return;

            float distance = Vector3.Distance(transform.position, _player.position);
            if (distance <= attackRange + 1f) // Extra 1f buffer so the hit connects fairly
            {
                // UPDATED: Now searches children for the HP script!
                PlayerStats pStats = _player.GetComponentInChildren<PlayerStats>();
                if (pStats != null) pStats.TakeDamage(Mathf.RoundToInt(damage));
                Debug.Log(gameObject.name + " dealt " + damage + " damage to player!");
            }
        }
    }

    // SPAWNER INTEGRATION: Called by EnemySpawner to increase difficulty
    public void ScaleDifficulty(float timeMultiplier)
    {
        maxHealth *= timeMultiplier;
        _currentHealth = maxHealth;
        damage *= timeMultiplier;
        moveSpeed += (timeMultiplier * 0.5f);
    }
}