
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 10;
    public float lifeTime = 3f;

    private Vector2 direction;

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(
            direction * speed * Time.deltaTime,
            Space.World
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        Health enemyHealth = other.GetComponent<Health>();

        if (enemyHealth == null)
            enemyHealth = other.GetComponentInParent<Health>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
