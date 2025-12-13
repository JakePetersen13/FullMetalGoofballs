using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    [Header("---Health---")]
    public float HP = 50f;

    [Header("---References---")]
    public Rigidbody rb;
    public Rigidbody ragdollHips;
    public Barbecue targetBarbecue; // The enemy's barbecue to attack

    [Header("---AI Settings---")]
    public bool canLunge = true;
    public float detectionRange = 10f;
    public float lungeRange = 5f;
    public float moveSpeed = 8f;

    [Header("---AI Attack---")]
    public float baseLungeForce = 15f;
    public float aiLungeDuration = 0.3f;
    public float aiLungeCooldown = 2f;

    [Header("---Weapon---")]
    public WeaponData weapon;

    private float aiLungeForce;
    private float aiLungeDamage;

    [Header("---Physics---")]
    public float gravityForce = 15f;
    public float ragdollRotationTorque = 50f;

    private Transform nearestEnemy;
    private bool isLunging = false;
    private float lungeTimer = 0f;
    private float cooldownTimer = 0f;
    private bool hasDealtDamage = false;

    // Death event
    public event Action<PlayerAI> OnDeath;
    public bool isDead = false;

    void Start()
    {
        // Find enemy's barbecue if not assigned
        if (targetBarbecue == null)
        {
            Barbecue[] allBarbecues = FindObjectsOfType<Barbecue>();
            foreach (Barbecue bbq in allBarbecues)
            {
                if (bbq.team == Team.Enemy)
                {
                    targetBarbecue = bbq;
                    break;
                }
            }

            if (targetBarbecue == null)
            {
                Debug.LogWarning("PlayerAI: No enemy barbecue found!");
            }
        }

        // Setup weapon stats
        if (weapon != null)
        {
            aiLungeForce = baseLungeForce * weapon.lungeForceMultiplier;
            aiLungeDamage = weapon.damage;
            moveSpeed = moveSpeed * weapon.speedMultiplier;
            Debug.Log($"{gameObject.name} equipped {weapon.weaponName}: Damage={aiLungeDamage}, Speed={moveSpeed}");
        }
        else
        {
            aiLungeForce = baseLungeForce;
            aiLungeDamage = 20f;
            Debug.LogWarning($"{gameObject.name}: No weapon assigned, using default stats");
        }
    }

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        ApplyGravity();
        UpdateLunge();

        if (!isLunging)
        {
            AIBehavior();
        }
    }

    void AIBehavior()
    {
        // Find nearest enemy
        FindNearestEnemy();

        Transform currentTarget = null;
        float distanceToEnemy = float.MaxValue;
        float distanceToBarbecue = float.MaxValue;

        // Calculate distances
        if (nearestEnemy != null)
        {
            distanceToEnemy = Vector3.Distance(transform.position, nearestEnemy.position);
        }

        if (targetBarbecue != null)
        {
            distanceToBarbecue = Vector3.Distance(transform.position, targetBarbecue.transform.position);
        }

        // Prioritize enemy if in detection range, otherwise go for barbecue
        if (nearestEnemy != null && distanceToEnemy <= detectionRange)
        {
            currentTarget = nearestEnemy;

            // Rotate towards enemy
            RotateTowards(nearestEnemy.position);

            // Check if should lunge at enemy
            if (canLunge && distanceToEnemy <= lungeRange && cooldownTimer <= 0f)
            {
                StartLunge();
            }
            // Move towards enemy if out of lunge range
            else if (distanceToEnemy > lungeRange)
            {
                MoveTowards(nearestEnemy.position);
            }
        }
        else if (targetBarbecue != null)
        {
            // No enemy in range, go after barbecue
            currentTarget = targetBarbecue.transform;

            // Rotate towards barbecue
            RotateTowards(targetBarbecue.transform.position);

            // Check if should lunge at barbecue
            if (canLunge && distanceToBarbecue <= lungeRange && cooldownTimer <= 0f)
            {
                StartLunge();
            }
            // Move towards barbecue if out of lunge range
            else if (distanceToBarbecue > lungeRange)
            {
                MoveTowards(targetBarbecue.transform.position);
            }
        }
    }

    void FindNearestEnemy()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        float closestDistance = float.MaxValue;
        nearestEnemy = null;

        foreach (EnemyController enemy in enemies)
        {
            if (enemy.isDead) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
    }

    void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        rb.AddForce(direction * moveSpeed, ForceMode.Acceleration);

        // Cap speed
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVelocity.magnitude > moveSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        }
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 lookDir = targetPosition - transform.position;
        lookDir.y = 0;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);

            // Rotate ragdoll
            if (ragdollHips != null)
            {
                Vector3 currentForward = ragdollHips.rotation * Vector3.forward;
                Vector3 targetForward = targetRotation * Vector3.forward;
                Vector3 axis = Vector3.Cross(currentForward, targetForward);
                float angle = Vector3.Angle(currentForward, targetForward);

                if (angle > 1f && angle < 120f)
                {
                    ragdollHips.AddTorque(axis.normalized * angle * ragdollRotationTorque * Time.fixedDeltaTime, ForceMode.Acceleration);
                }

                ragdollHips.angularVelocity *= 0.95f;
            }

            // Rotate main body
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 5f));
        }
    }

    void StartLunge()
    {
        isLunging = true;
        lungeTimer = aiLungeDuration;
        cooldownTimer = aiLungeCooldown;
        hasDealtDamage = false;

        Vector3 lungeDirection = transform.forward;
        rb.AddForce(lungeDirection * aiLungeForce, ForceMode.VelocityChange);
    }

    void UpdateLunge()
    {
        if (isLunging)
        {
            lungeTimer -= Time.fixedDeltaTime;

            if (lungeTimer <= 0f)
            {
                isLunging = false;
            }
        }
    }

    void ApplyGravity()
    {
        rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);
    }

    public void TakeDamage(float damage)
    {
        HP -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {HP}");

        if (HP <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"{gameObject.name} died!");

        OnDeath?.Invoke(this);
        canLunge = false;

        gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isLunging && !hasDealtDamage)
        {
            // Hit enemy
            if (collision.gameObject.CompareTag("Enemy"))
            {
                EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
                if (enemy != null && !enemy.isDead)
                {
                    enemy.TakeDamage(aiLungeDamage);
                    hasDealtDamage = true;
                    Debug.Log("PlayerAI hit enemy for " + aiLungeDamage + " damage!");
                }
            }
            // Hit enemy's barbecue
            else if (targetBarbecue != null && collision.gameObject == targetBarbecue.gameObject)
            {
                targetBarbecue.TakeDamage(aiLungeDamage, gameObject);
                hasDealtDamage = true;
                Debug.Log("PlayerAI hit enemy's barbecue for " + aiLungeDamage + " damage!");
            }
        }
    }
}