using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2.5f;
    public float stopDistance = 0.5f;
    public float chaseDistance = 6f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > stopDistance && distance <= chaseDistance)
        {
            Vector2 direction =
                ((Vector2)player.position - rb.position).normalized;

            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}