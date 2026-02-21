using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 360f;
    public float stoppingDistance = 6f; // Distance to keep from player

    [Header("Combat")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;

    private Transform _player;
    private Rigidbody _rb;
    private float _fireTimer;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (_player == null) return;

        // 1. Shooting Cooldown Logic
        if (_fireTimer > 0) _fireTimer -= Time.deltaTime;

        // 2. Automate "OnFire" behavior
        if (_fireTimer <= 0 && Vector3.Distance(transform.position, _player.position) <= stoppingDistance + 2f)
        {
            Shoot();
            _fireTimer = fireRate;
        }
    }

    void FixedUpdate()
    {
        if (_player == null) return;

        // 1. Calculate the direction to the player
        Vector3 direction = (_player.position - transform.position).normalized;

        // 2. Aim: Rotate to face the player every frame
        RotateTowards(new Vector2(direction.x, direction.z));

        // 3. Movement: Only move if outside stopping distance
        float distance = Vector3.Distance(transform.position, _player.position);
        if (distance > stoppingDistance)
        {
            _rb.linearVelocity = new Vector3(direction.x, 0, direction.z) * moveSpeed;
        }
        else
        {
            _rb.linearVelocity = Vector3.zero;
        }
    }

    void RotateTowards(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, angle, 0), rotationSpeed * Time.fixedDeltaTime);
    }

    void Shoot()
    {
        // Simple automated shoot to firePoint direction
        Instantiate(arrowPrefab, firePoint.position, transform.rotation);
    }
}