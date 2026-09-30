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

    private float nextAttackTime = 0f;


    private void Awake()
    {
        // Daño inicial
        if (baseDamage <= 0)
        {
            baseDamage = 10;
        }

        damage = baseDamage;
    }


    private void Update()
    {
        // Clic izquierdo
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
        // Evita atacar demasiado rápido
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime =
            Time.time + attackCooldown;


        if (attackPoint == null)
        {
            Debug.LogWarning(
                "PlayerCombat: attackPoint no está asignado."
            );

            return;
        }


        // Busca enemigos dentro del círculo
        Collider2D[] hitEnemies =
            Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayer
            );


        // Si no encontró enemigos
        if (hitEnemies.Length == 0)
        {
            Debug.Log("No golpeaste a ningún enemigo.");
            return;
        }


        // Daño a los enemigos encontrados
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
    // ACTUALIZAR DAÑO
    // =====================================================

    public void UpdateDamage(int totalBonus)
    {
        damage =
            baseDamage +
            totalBonus;
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


        // Recalcular daño incluyendo equipamiento
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
    // VER RANGO DE ATAQUE EN LA SCENE
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