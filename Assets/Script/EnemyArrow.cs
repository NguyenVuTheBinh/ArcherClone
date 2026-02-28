using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
    public float speed = 15f; // Generally slower than player arrows
    public int damage = 10;
    public bool canRicochet = false;
    private int bounceCount = 0;
    private int maxBounces = 1;
    public string tagToIgnore;
    
    Rigidbody rb;

    void Start()
    {
        // Find all objects with the tags we want to ignore
        GameObject[] objectsToIgnore = GameObject.FindGameObjectsWithTag(tagToIgnore);
        foreach (GameObject obj in objectsToIgnore)
        {
            Collider enemyCollider = obj.GetComponent<Collider>();
            Collider bulletCollider = GetComponent<Collider>();

            if (enemyCollider != null && bulletCollider != null)
            {
                // Tell Unity's 3D physics engine to ignore this specific pair
                Physics.IgnoreCollision(bulletCollider, enemyCollider);
            }
        }

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