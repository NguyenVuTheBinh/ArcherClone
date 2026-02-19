using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1; // This will be set by the player when shooting
    public bool canRicochet = false;
    private int bounceCount = 0;
    private int maxBounces = 2;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Launch the bullet
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, 5f);
    }

    void FixedUpdate()
    {
        // If the bullet has moved, make it face the direction it is traveling
        if (rb.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. Check if we hit an Enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Uses the upgraded damage!
            }
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (canRicochet && bounceCount < maxBounces)
            {
                bounceCount++;
                // We don't destroy, the Physic Material handles the bounce direction
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
    }
}