using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    public int baseDamage = 10;
    public int damage = 10;

    [Header("Ataque")]
    public float attackRange = 0.8f;
    public Transform attackPoint;
    public LayerMask enemyLayer;

    [Header("Tiempo entre ataques")]
    public float attackCooldown = 0.4f;

    [Header("Animación")]
    public Animator animator;

    private float nextAttackTime = 0f;

    private void Awake()
    {
        if (baseDamage <= 0)
        {
            baseDamage = 10;
        }

        damage = baseDamage;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    // =====================================================
    // ATAQUE
    // =====================================================

    private void Attack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
        }

        if (attackPoint == null)
        {
            Debug.LogWarning(
                "PlayerCombat: attackPoint no está asignado."
            );

            if (animator != null)
            {
                animator.SetBool("isAttacking", false);
            }

            return;
        }

        Collider2D[] hitEnemies =
            Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayer
            );

        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth =
                enemy.GetComponent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);

                Debug.Log(
                    "Pegaste " +
                    damage +
                    " de daño a " +
                    enemy.gameObject.name
                );
            }
        }
    }

    // =====================================================
    // TERMINAR ANIMACIÓN
    // =====================================================

    public void FinishAttacking()
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    // =====================================================
    // ACTUALIZAR DAÑO
    // =====================================================

    public void UpdateDamage(int totalBonus)
    {
        damage = baseDamage + totalBonus;
    }

    // =====================================================
    // DAÑO POR SUBIR DE NIVEL
    // =====================================================

    public void AddLevelDamage(int amount)
    {
        baseDamage += amount;

        Debug.Log(
            "Daño base aumentado: " +
            baseDamage
        );

        if (EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.RecalculateDamage();
        }
        else
        {
            damage = baseDamage;
        }
    }

    // =====================================================
    // GIZMO
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange
        );
    }
}