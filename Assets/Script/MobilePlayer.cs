using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MobilePlayer : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 720f;

    [Header("Combat")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float baseFireRate = 1f; // <--- NEW: Seconds between shots

    [Header("References")]
    public PlayerStats playerStats;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Rigidbody _rb;
    private float _fireTimer; // <--- NEW: Tracks cooldown

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (playerStats == null) playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        // Update the cooldown timer
        if (_fireTimer > 0) _fireTimer -= Time.deltaTime;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        // Only shoot if cooldown is over
        if (context.started && _fireTimer <= 0)
        {
            if (arrowPrefab != null && firePoint != null)
            {
                Shoot();
                // Reset timer based on attack speed stat
                float speedMod = (playerStats != null) ? playerStats.attackSpeedModifier : 1f;
                _fireTimer = baseFireRate / speedMod;
            }
        }
    }

    void Shoot()
    {
        int multishot = 1 + (playerStats != null ? playerStats.multishotCount : 0);
        int frontArrows = (playerStats != null ? playerStats.frontArrowCount : 0);

        float spreadAngle = 15f;
        float startAngle = -(spreadAngle * (multishot - 1)) / 2;

        // Loop for Front Arrows (depth)
        for (int f = 0; f <= frontArrows; f++)
        {
            Vector3 offset = transform.forward * (f * 0.4f); // Spacing between front arrows

            // Loop for Multishot (spread)
            for (int i = 0; i < multishot; i++)
            {
                float currentAngle = startAngle + (i * spreadAngle);
                Quaternion rotation = transform.rotation * Quaternion.Euler(0, currentAngle, 0);

                GameObject arrow = Instantiate(arrowPrefab, firePoint.position + offset, rotation);

                ArrowProjectile script = arrow.GetComponent<ArrowProjectile>();
                if (script != null && playerStats != null)
                {
                    script.damage = playerStats.damage;
                    script.canRicochet = playerStats.hasRicochet;
                }
            }
        }
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector3(_moveInput.x, 0, _moveInput.y) * moveSpeed;
        if (_lookInput.sqrMagnitude > 0.05f) RotateTowards(_lookInput);
        else if (_moveInput.sqrMagnitude > 0.05f) RotateTowards(_moveInput);
    }

    void RotateTowards(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, angle, 0), rotationSpeed * Time.fixedDeltaTime);
    }
}