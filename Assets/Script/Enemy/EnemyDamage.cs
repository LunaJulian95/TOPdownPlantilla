using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage = 10;
    public float cooldown = 1f;
    private float lastAttackTime;

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time > lastAttackTime + cooldown)
            {
                collision.gameObject.GetComponent<Health>().TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }
}