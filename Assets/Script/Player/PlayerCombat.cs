
using UnityEngine;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    public int baseDamage = 10;
    public int damage = 10;

    [Header("Ataque cuerpo a cuerpo")]
    public float attackRange = 0.8f;
    public Transform attackPoint;
    public LayerMask enemyLayer;

    [Header("Tiempo entre ataques")]
    public float attackCooldown = 0.4f;

    [Header("Animación")]
    public Animator animator;

    [Header("Proyectil")]
    public Transform firePoint;

    [Header("Ataque en área")]
    public float areaRadius = 1.5f;
    public GameObject areaEffectPrefab;

    [Header("Aura")]
    public float auraRadius = 1.5f;
    public float auraDamageInterval = 1f;
    public GameObject auraEffectPrefab;

    private float nextAttackTime = 0f;
    private float nextAuraDamageTime = 0f;

    private GameObject activeAuraEffect;
    private Item activeAuraWeapon;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (baseDamage <= 0)
            baseDamage = 10;

        damage = baseDamage;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAura();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    // =====================================================
    // ATAQUE PRINCIPAL
    // =====================================================

    private void Attack()
    {
        if (Time.time < nextAttackTime)
            return;

        Item weapon = null;

        if (EquipmentManager.Instance != null)
            weapon = EquipmentManager.Instance.currentWeapon;

        // Sin arma: ataque cuerpo a cuerpo
        if (weapon == null)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartAttackAnimation();
            MeleeAttack();
            return;
        }

        switch (weapon.attackType)
        {
            case Item.AttackType.Melee:

                nextAttackTime = Time.time + attackCooldown;
                StartAttackAnimation();
                MeleeAttack();
                break;

            case Item.AttackType.Projectile:

                if (weapon.projectilePrefab == null)
                {
                    Debug.LogWarning("El arma no tiene Projectile Prefab.");
                    return;
                }

                nextAttackTime = Time.time + attackCooldown;
                StartAttackAnimation();
                ShootProjectile(weapon);
                break;

            case Item.AttackType.Area:

                nextAttackTime = Time.time + attackCooldown;
                StartAttackAnimation();
                AreaAttack(weapon);
                break;

            case Item.AttackType.Aura:

                // La aura ataca automáticamente.
                break;
        }
    }

    private void StartAttackAnimation()
    {
        if (animator != null)
            animator.SetBool("isAttacking", true);
    }

    // =====================================================
    // ATAQUE CUERPO A CUERPO
    // =====================================================

    private void MeleeAttack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("PlayerCombat: attackPoint no asignado.");
            return;
        }

        Collider2D[] hitEnemies =
            Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayer
            );

        HashSet<Health> damagedEnemies = new HashSet<Health>();

        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();

            if (enemyHealth == null)
                enemyHealth = enemy.GetComponentInParent<Health>();

            if (enemyHealth != null && damagedEnemies.Add(enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log("Daño cuerpo a cuerpo: " + damage);
            }
        }
    }

    // =====================================================
    // ATAQUE CON PROYECTIL
    // =====================================================

    private void ShootProjectile(Item weapon)
    {
        if (firePoint == null)
        {
            Debug.LogWarning("PlayerCombat: firePoint no asignado.");
            return;
        }

        Vector2 direction = playerMovement != null
            ? playerMovement.GetLastMoveDirection()
            : Vector2.down;

        GameObject projectile = Instantiate(
            weapon.projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        PlayerProjectile projectileScript =
            projectile.GetComponent<PlayerProjectile>();

        if (projectileScript == null)
        {
            Debug.LogError("El prefab no tiene PlayerProjectile.");
            Destroy(projectile);
            return;
        }

        projectileScript.speed = weapon.projectileSpeed;
        projectileScript.damage = damage;
        projectileScript.SetDirection(direction);

        Debug.Log("Proyectil disparado: " + direction);
    }

    // =====================================================
    // ATAQUE EN ÁREA
    // =====================================================

    private void AreaAttack(Item weapon)
    {
        if (weapon == null)
            return;

        Vector2 attackPosition = transform.position;

        float radius = weapon.areaRadius;

        // Si el arma no tiene un radio válido, usar el del jugador.
        if (radius <= 0)
            radius = areaRadius;

        GameObject effect = weapon.areaEffectPrefab != null
            ? weapon.areaEffectPrefab
            : areaEffectPrefab;

        if (effect != null)
        {
            Instantiate(
                effect,
                attackPosition,
                Quaternion.identity
            );
        }

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackPosition,
            radius,
            enemyLayer
        );

        HashSet<Health> damagedEnemies = new HashSet<Health>();

        foreach (Collider2D enemy in enemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();

            if (enemyHealth == null)
                enemyHealth = enemy.GetComponentInParent<Health>();

            if (enemyHealth != null && damagedEnemies.Add(enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        Debug.Log("Ataque de área. Enemigos detectados: " + damagedEnemies.Count);
    }

    // =====================================================
    // AURA AUTOMÁTICA
    // =====================================================

    private void UpdateAura()
    {
        if (EquipmentManager.Instance == null)
            return;

        Item weapon = EquipmentManager.Instance.currentWeapon;

        // Si no hay arma equipada, desactivar el aura.
        if (weapon == null)
        {
            if (activeAuraEffect != null || activeAuraWeapon != null)
            {
                Debug.LogWarning("AURA: EquipmentManager no tiene arma equipada.");
                DeactivateAura();
            }

            return;
        }

        // Si el arma no tiene aura, desactivarla.
        if (weapon.attackType != Item.AttackType.Aura)
        {
            if (activeAuraEffect != null || activeAuraWeapon != null)
            {
                Debug.Log("AURA: El arma equipada no es de tipo Aura: " + weapon.itemName);
                DeactivateAura();
            }

            return;
        }

        // Crear el efecto solamente si todavía no existe.
        if (activeAuraEffect == null)
        {
            activeAuraWeapon = weapon;

            if (auraEffectPrefab == null)
            {
                Debug.LogError("AURA: Falta asignar auraEffectPrefab en PlayerCombat.");
                return;
            }

            activeAuraEffect = Instantiate(auraEffectPrefab, transform, false);

            activeAuraEffect.transform.localPosition = Vector3.zero;
            activeAuraEffect.transform.localRotation = Quaternion.identity;
            activeAuraEffect.transform.localScale = Vector3.one * 2f;

            Debug.Log("AURA: Efecto creado correctamente.");
        }

        // Aplicar daño periódicamente.
        if (Time.time >= nextAuraDamageTime)
        {
            ApplyAuraDamage();
            nextAuraDamageTime = Time.time + auraDamageInterval;
        }
    }
    private void ApplyAuraDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position,
            auraRadius,
            enemyLayer
        );

        HashSet<Health> damagedEnemies = new HashSet<Health>();

        foreach (Collider2D enemy in enemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();

            if (enemyHealth == null)
                enemyHealth = enemy.GetComponentInParent<Health>();

            if (enemyHealth != null && damagedEnemies.Add(enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
            }
        }
    }

    private void DeactivateAura()
    {
        Debug.Log("DESACTIVANDO AURA");

        if (activeAuraEffect != null)
        {
            Destroy(activeAuraEffect);
            activeAuraEffect = null;
        }

        activeAuraWeapon = null;
    }

    // =====================================================
    // TERMINAR ANIMACIÓN
    // =====================================================

    public void FinishAttacking()
    {
        if (animator != null)
            animator.SetBool("isAttacking", false);
    }

    // =====================================================
    // ACTUALIZAR DAÑO
    // =====================================================

    public void UpdateDamage(int totalBonus)
    {
        damage = baseDamage + totalBonus;
    }

    public void AddLevelDamage(int amount)
    {
        baseDamage += amount;

        Debug.Log("Daño base aumentado: " + baseDamage);

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
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(
                attackPoint.position,
                attackRange
            );
        }

        Gizmos.DrawWireSphere(
            transform.position,
            auraRadius
        );
    }
}
