using System;
using System.Collections;
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
    public Barbecue ownBarbecue;

    [Header("---AI Settings---")]
    public bool canLunge = true;
    public float detectionRange = 10f;
    public float lungeRange = 5f;
    public float moveSpeed = 8f;
    public float barbecueDefenseRadius = 8f; // Distance to collapse to barbecue when threats nearby

    [Header("---AI Attack---")]
    public float baseLungeForce = 15f;
    public float aiLungeDuration = 0.3f;
    public float aiLungeCooldown = 2f;
    public GameObject impactParticlePrefab; // Impact effect when hitting something

    [Header("---Weapon---")]
    public WeaponData weapon;

    private float aiLungeForce;
    private float aiLungeDamage;

    [Header("---Physics---")]
    public float gravityForce = 15f;
    public float ragdollRotationTorque = 50f;

    [Header("---Audio---")]
    public AudioPlayerHelper audioPlayer;
    public AudioSource audioSource;
    public AudioClip punch1;
    public AudioClip punch2;
    public AudioClip deathSound;
    public AudioClip BBQHit;
    public float minPitchVariation = 0.6f;
    public float maxPitchVariation = 1.4f;
    public float deathSoundDelay = 1.5f; // How long to wait before deactivating after death sound

    private Transform nearestEnemy;
    public bool isLunging = false;
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
                else
                {
                    ownBarbecue = bbq;
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

        // Check if enemy is near our barbecue (defensive collapse)
        bool shouldDefendBarbecue = false;
        Barbecue ourBarbecue = FindOurBarbecue();
        if (ourBarbecue != null && nearestEnemy != null)
        {
            float enemyDistanceToOurBBQ = Vector3.Distance(nearestEnemy.position, ourBarbecue.transform.position);
            if (enemyDistanceToOurBBQ <= barbecueDefenseRadius)
            {
                shouldDefendBarbecue = true;
            }
        }

        // Priority 1: Defend our barbecue if enemy is nearby
        if (shouldDefendBarbecue && ourBarbecue != null)
        {
            float distanceToOurBBQ = Vector3.Distance(transform.position, ourBarbecue.transform.position);

            // Rotate towards our barbecue
            RotateTowards(ourBarbecue.transform.position);

            // Move to our barbecue if not already there
            if (distanceToOurBBQ > 2f)
            {
                MoveTowards(ourBarbecue.transform.position);
            }

            // Attack enemy if in range while defending
            if (canLunge && distanceToEnemy <= lungeRange && cooldownTimer <= 0f)
            {
                RotateTowards(nearestEnemy.position);
                StartLunge();
            }
        }
        // Priority 2: Prioritize enemy if in detection range
        else if (nearestEnemy != null && distanceToEnemy <= detectionRange)
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
        // Priority 3: No enemy in range, go after barbecue
        else if (targetBarbecue != null)
        {
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

    Barbecue FindOurBarbecue()
    {
        // Find the player team's barbecue (our barbecue)
        Barbecue[] allBarbecues = FindObjectsOfType<Barbecue>();
        foreach (Barbecue bbq in allBarbecues)
        {
            if (bbq.team == Team.Player)
            {
                return bbq;
            }
        }
        return null;
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

        // Play death sound and wait before deactivating
        PlayDeathScream();

        // Start coroutine to deactivate after sound plays
        StartCoroutine(DeactivateAfterDeath());
    }

    IEnumerator DeactivateAfterDeath()
    {
        // Wait for the death sound to finish playing
        yield return new WaitForSeconds(deathSoundDelay);

        // Now deactivate the GameObject
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

                    PlayRandomPunchSound();
                    // Spawn impact particle at collision point
                    SpawnImpactEffect(collision);

                    Debug.Log("PlayerAI hit enemy for " + aiLungeDamage + " damage!");
                }
            }
            // Hit enemy's barbecue
            else if (targetBarbecue != null && collision.gameObject == targetBarbecue.gameObject)
            {
                SpawnImpactEffect(collision);
                audioSource.PlayOneShot(BBQHit);
            }
        }
    }

    void PlayRandomPunchSound()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("Audio Source not assigned!");
            return;
        }

        // Randomly select punch1 or punch2
        AudioClip selectedClip = UnityEngine.Random.value > 0.5f ? punch1 : punch2;

        if (selectedClip != null)
        {
            // Randomize pitch within the specified range
            audioSource.pitch = UnityEngine.Random.Range(minPitchVariation, maxPitchVariation);

            // Play the sound
            audioSource.PlayOneShot(selectedClip);
        }
        else
        {
            Debug.LogWarning("Punch audio clips not assigned!");
        }
    }

    void PlayDeathScream()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("Audio Source not assigned!");
            return;
        }

        if (deathSound != null)
        {
            audioSource.pitch = UnityEngine.Random.Range(minPitchVariation, maxPitchVariation);
            audioSource.PlayOneShot(deathSound);
        }
        else
        {
            Debug.LogWarning("Death sound not assigned!");
        }
    }

    void SpawnImpactEffect(Collision collision)
    {
        if (impactParticlePrefab != null && collision.contactCount > 0)
        {
            // Get the contact point
            ContactPoint contact = collision.GetContact(0);

            // Spawn particle at contact point
            GameObject impact = Instantiate(impactParticlePrefab, contact.point, Quaternion.LookRotation(contact.normal));

            // Destroy after particle duration (default 2 seconds)
            Destroy(impact, 2f);
        }
    }
}