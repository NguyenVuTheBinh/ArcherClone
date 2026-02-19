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

    [Header("References")]
    // This connects to your upgrade system logic
    public PlayerStats playerStats;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Automatically find player stats if not dragged in the inspector
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
        }
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
        // Only shoot once when the button is first pressed
        if (context.started)
        {
            if (arrowPrefab != null && firePoint != null)
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        // 1. GET CURRENT STATS FROM UPGRADES
        int arrowCount = 1;
        bool ricochet = false;
        int currentDamage = 1;

        if (playerStats != null)
        {
            // Multishot adds extra arrows
            arrowCount += playerStats.multishotCount;
            // Ricochet enables bouncing
            ricochet = playerStats.hasRicochet;
            // Damage uses the upgraded stat
            currentDamage = playerStats.damage;
        }

        // 2. CALCULATE SPREAD (Cone shape)
        float spreadAngle = 15f;
        float startAngle = -(spreadAngle * (arrowCount - 1)) / 2;

        // 3. SPAWN LOOP
        for (int i = 0; i < arrowCount; i++)
        {
            float currentAngle = startAngle + (i * spreadAngle);
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, currentAngle, 0);

            GameObject newArrow = Instantiate(arrowPrefab, firePoint.position, rotation);

            // 4. APPLY STATS TO THE PROJECTILE
            ArrowProjectile arrowScript = newArrow.GetComponent<ArrowProjectile>();
            if (arrowScript != null)
            {
                // Set the damage based on your DamageUp upgrades
                arrowScript.damage = currentDamage;
                // Enable bouncing if Ricochet was picked
                if (ricochet) arrowScript.canRicochet = true;
            }
        }
    }

    void FixedUpdate()
    {
        // Move the player using Rigidbody for better physics interactions
        Vector3 targetVelocity = new Vector3(_moveInput.x, 0, _moveInput.y) * moveSpeed;
        _rb.linearVelocity = targetVelocity;
        RotateTowards(_lookInput);
        // Rotate towards movement or look direction
        if (_lookInput.sqrMagnitude > 0.05f)
        {
            
        }
        else if (_moveInput.sqrMagnitude > 0.05f)
        {
            RotateTowards(_moveInput);
        }
    }

    void RotateTowards(Vector2 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}