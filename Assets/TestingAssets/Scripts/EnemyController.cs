using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("---Health---")]
    public float HP = 50f;

    [Header("---References---")]
    public Rigidbody rb;
    public Rigidbody ragdollHips;

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
    public WeaponData enemyWeapon;

    private float aiLungeForce;
    private float aiLungeDamage;

    [Header("---Physics---")]
    public float gravityForce = 15f;
    public float ragdollRotationTorque = 50f;

    private Transform player;
    private bool isLunging = false;
    private float lungeTimer = 0f;
    private float cooldownTimer = 0f;
    private bool hasDealtDamage = false;

    void Start()
    {
        // Find player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("Enemy: No player found with 'Player' tag!");
        }

        // Setup weapon stats
        if (enemyWeapon != null)
        {
            aiLungeForce = baseLungeForce * enemyWeapon.lungeForceMultiplier;
            aiLungeDamage = enemyWeapon.damage;
            moveSpeed = moveSpeed * enemyWeapon.speedMultiplier;
            Debug.Log($"{gameObject.name} equipped {enemyWeapon.weaponName}: Damage={aiLungeDamage}, Speed={moveSpeed}");
        }
        else
        {
            // Default values if no weapon assigned
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
        if (player == null) return;

        ApplyGravity();
        UpdateLunge();

        if (!isLunging)
        {
            AIBehavior();
        }
    }

    void AIBehavior()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Rotate towards player
        RotateTowardsPlayer();

        // Check if should lunge
        if (canLunge && distanceToPlayer <= lungeRange && cooldownTimer <= 0f)
        {
            StartLunge();
        }
        // Move towards player if out of lunge range but in detection range
        else if (distanceToPlayer > lungeRange && distanceToPlayer <= detectionRange)
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
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

    void RotateTowardsPlayer()
    {
        Vector3 lookDir = player.position - transform.position;
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

    // Called when enemy is hit during player lunge
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
        Debug.Log($"{gameObject.name} died!");
        gameObject.SetActive(false);
        // Or: Destroy(gameObject);
    }

    // Detect collision with player during lunge
    void OnCollisionEnter(Collision collision)
    {
        if (isLunging && !hasDealtDamage && collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(aiLungeDamage);
                hasDealtDamage = true;
                Debug.Log("Enemy hit player for " + aiLungeDamage + " damage!");
            }
        }
    }
}