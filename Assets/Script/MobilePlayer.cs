using UnityEngine;
using Terresquall; // Ensure this namespace is here

[RequireComponent(typeof(Rigidbody))]
public class MobilePlayer : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 720f;

    [Header("Joystick IDs")]
    public int moveJoystickID = 0; // Left Joystick
    public int aimJoystickID = 1;  // Right Joystick

    [Header("Combat")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float baseFireRate = 1f;

    [Header("References")]
    public PlayerStats playerStats;

    private Rigidbody _rb;
    private float _fireTimer;

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
        if (_fireTimer > 0) _fireTimer -= Time.deltaTime;

        // AUTOMATIC SHOOTING: Reads Aiming Joystick (ID 1)
        Vector2 aimInput = VirtualJoystick.GetAxis(aimJoystickID);
        if (aimInput.sqrMagnitude > 0.2f && _fireTimer <= 0)
        {
            Shoot();
            ResetFireTimer();
        }
    }

    void FixedUpdate()
    {
        // 1. MOVEMENT: Reads Movement Joystick (ID 0)
        Vector2 moveInput = VirtualJoystick.GetAxis(moveJoystickID);
        _rb.linearVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;

        // 2. ROTATION: Prioritize Aiming over Movement
        Vector2 aimInput = VirtualJoystick.GetAxis(aimJoystickID);

        if (aimInput.sqrMagnitude > 0.05f)
        {
            RotateTowards(aimInput);
        }
        else if (moveInput.sqrMagnitude > 0.05f)
        {
            RotateTowards(moveInput);
        }
    }

    void ResetFireTimer()
    {
        float speedMod = (playerStats != null) ? playerStats.attackSpeedModifier : 1f;
        _fireTimer = baseFireRate / speedMod;
    }

    void Shoot()
    {
        int multishot = 1 + (playerStats != null ? playerStats.multishotCount : 0);
        int frontArrows = (playerStats != null ? playerStats.frontArrowCount : 0);

        float spreadAngle = 15f;
        float startAngle = -(spreadAngle * (multishot - 1)) / 2;

        for (int f = 0; f <= frontArrows; f++)
        {
            Vector3 offset = transform.forward * (f * 0.4f);
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

    void RotateTowards(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, angle, 0), rotationSpeed * Time.fixedDeltaTime);
    }
}