using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    public float speed = 15f; // Generally slower than player arrows
    public int damage = 10;
    public bool canRicochet = false;
    private int bounceCount = 0;
    private int maxBounces = 1;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Launch the arrow forward
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, 5f);
    }

    void FixedUpdate()
    {
        // Make the arrow face its travel direction
        if (rb.linearVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. Check if the enemy hit the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(damage); // Deal damage to the player
            }
            Destroy(gameObject);
        }

        // 2. Wall Bounce/Ricochet Logic
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (canRicochet && bounceCount < maxBounces)
            {
                bounceCount++;
                // Let Physics Material handles the bounce direction
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}