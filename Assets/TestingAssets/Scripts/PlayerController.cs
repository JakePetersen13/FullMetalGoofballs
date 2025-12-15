using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("---Physics---")]
    public float baseMaxSpeed = 15f;
    public float accelerationRate = 5f;
    public float gravityForce = 15f;

    [Header("---References---")]
    public Rigidbody rb;
    public Camera mainCamera;

    [Header("---Ragdoll---")]
    public Rigidbody ragdollHips;
    public float ragdollRotationTorque;
    public Vector3 weaponPositionOffset = Vector3.zero;
    public Vector3 weaponRotationOffset = Vector3.zero;
    public Vector3 weaponScaleOverride = Vector3.one; // Force weapon scale

    [Header("---Attack---")]
    public float baseLungeForce = 20f;
    public float lungeDuration = 0.3f;
    public float lungeCooldown = 0.5f;

    [Header("---Weapon---")]
    public WeaponData currentWeapon;

    public bool isLunging = false;
    private float lungeTimer = 0f;
    private float cooldownTimer = 0f;
    private bool hasDealtDamage = false;
    private GameObject equippedWeaponModel;

    [Header("---Player---")]
    public float HP = 100f;
    public float maxHP = 100f;
    public bool canMove = true; // Toggle to disable/enable movement

    [Header("---Audio---")]
    public AudioPlayerHelper audioPlayer;
    public AudioSource audioSource;
    public AudioClip punch1;
    public AudioClip punch2;
    public AudioClip deathSound;
    public AudioClip BBQHit;
    [Range(0.8f, 1.2f)]
    public float minPitchVariation = 0.9f;
    [Range(0.8f, 1.2f)]
    public float maxPitchVariation = 1.1f;
    public float deathSoundDelay = 1.5f; // How long to wait before deactivating after death sound

    [Header("---Particle---")]
    public GameObject impactParticlePrefab; // Impact effect when hitting something
    public GameObject hurtParticlePrefab;
    public GameObject deathParticlePrefab;
    public DamageFlashUI damageFlashUI;

    // Calculated values based on weapon
    private float maxSpeed;
    private float lungeForce;
    private float lungeDamage;

    public bool isDead = false;
    void Start()
    {
        EquipWeapon(currentWeapon);
    }

    void Update()
    {
        if (canMove)
        {
            HandleLungeInput();
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            HandleMovement();
            RotateTowardsMouse();
        }
        UpdateLunge();
        checkHP();
    }

    // Equip a weapon and update stats
    public void EquipWeapon(WeaponData weapon)
    {
        if (weapon == null)
        {
            Debug.LogWarning("No weapon data provided!");
            return;
        }

        currentWeapon = weapon;

        // Calculate stats based on weapon
        maxSpeed = baseMaxSpeed * weapon.speedMultiplier;
        lungeForce = baseLungeForce * weapon.lungeForceMultiplier;
        lungeDamage = weapon.damage;

        Debug.Log($"Equipped {weapon.weaponName}: Damage={lungeDamage}, Speed={maxSpeed}, LungeForce={lungeForce}");
    }

    void HandleLungeInput()
    {
        if (Input.GetMouseButtonDown(0) && !isLunging && cooldownTimer <= 0f)
        {
            StartLunge();
        }

        // Update cooldown
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void StartLunge()
    {
        isLunging = true;
        lungeTimer = lungeDuration;
        cooldownTimer = lungeCooldown;
        hasDealtDamage = false;

        // Apply instant forward force (uses weapon-modified lungeForce)
        Vector3 lungeDirection = transform.forward;
        rb.AddForce(lungeDirection * lungeForce, ForceMode.VelocityChange);
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

    void HandleMovement()
    {
        //-----MOVEMENT-----
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // Reduce movement control during lunge
        float movementMultiplier = isLunging ? 0.2f : 1f;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            // Applies force based on moveDir vector
            moveDir.Normalize();
            rb.AddForce(moveDir * accelerationRate * movementMultiplier, ForceMode.Acceleration);

            // maxSpeed cap (uses weapon-modified maxSpeed)
            if (rb.velocity.magnitude > maxSpeed && !isLunging)
                rb.velocity = rb.velocity.normalized * maxSpeed;

        }
        else
        {
            // deceleration (reduced during lunge to maintain momentum)
            if (rb.velocity.magnitude > 0.01f && !isLunging)
            {
                Vector3 decelDir = -rb.velocity.normalized;
                rb.AddForce(decelDir * accelerationRate, ForceMode.Acceleration);

                if (rb.velocity.magnitude < 0.1f)
                    rb.velocity = Vector3.zero;
            }
        }

        // Gravity
        rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);
    }

    // rotates player model towards the mouse, based on its position, given by main camera
    void RotateTowardsMouse()
    {
        // Don't rotate during lunge to maintain direction
        if (isLunging) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0;

            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                RotateRagdollTowards(targetRotation);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 1f));
            }
        }
    }

    void RotateRagdollTowards(Quaternion targetRot)
    {
        Vector3 currentForward = ragdollHips.rotation * Vector3.forward;
        Vector3 targetForward = targetRot * Vector3.forward;
        Vector3 axis = Vector3.Cross(currentForward, targetForward);
        float angle = Vector3.Angle(currentForward, targetForward);

        if (angle > 1f && angle < 120f)
        {
            ragdollHips.AddTorque(axis.normalized * angle * ragdollRotationTorque * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        ragdollHips.angularVelocity *= 0.95f;
    }

    void checkHP()
    {
        if (HP <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player died!");
        canMove = false;

        // Play death sound and wait before deactivating
        PlayDeathScream();
        damageFlashUI.KeepOn();

        GameObject death = Instantiate(
            deathParticlePrefab,
            transform.position,
            Quaternion.identity
        );

        // Start coroutine to deactivate after sound plays
        StartCoroutine(DeactivateAfterDeath());

        Destroy(death, 2f);
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

    IEnumerator DeactivateAfterDeath()
    {
        // Wait for the death sound to finish playing
        yield return new WaitForSeconds(deathSoundDelay);

        // Now deactivate the GameObject
        this.gameObject.SetActive(false);
    }

    // Called by enemies when they hit the player
    public void TakeDamage(float damage)
    {
        HP -= damage;
        HP = Mathf.Clamp(HP, 0f, maxHP);
        Debug.Log($"Player took {damage} damage. HP: {HP}");

        // Hurt effect in front of camera
        float distanceFromCamera = 0.5f;

        Vector3 hurtPos = mainCamera.transform.position +
                          mainCamera.transform.forward * distanceFromCamera;

        damageFlashUI.Flash();

        GameObject hurt = Instantiate(
            hurtParticlePrefab,
            hurtPos,
            Quaternion.identity
        );

        // Parent to camera so it follows view
        hurt.transform.SetParent(mainCamera.transform);

        // Face the camera (for quads / particles)
        hurt.transform.LookAt(mainCamera.transform);

        Destroy(hurt, 2f);
    }

    // Detect collision with enemies during lunge
    void OnCollisionEnter(Collision collision)
    {
        if (isLunging && !hasDealtDamage && collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("BBQ"))
            {
                SpawnImpactEffect(collision);
                audioSource.PlayOneShot(BBQHit);
            }
            else
            {
                EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(lungeDamage);
                    hasDealtDamage = true;

                    SpawnImpactEffect(collision);
                    PlayRandomPunchSound();
                    Debug.Log($"Player hit enemy for {lungeDamage} damage with {currentWeapon.weaponName}!");
                }
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
        AudioClip selectedClip = Random.value > 0.5f ? punch1 : punch2;

        if (selectedClip != null)
        {
            // Randomize pitch within the specified range
            audioSource.pitch = Random.Range(minPitchVariation, maxPitchVariation);

            // Play the sound
            audioSource.PlayOneShot(selectedClip);
        }
        else
        {
            Debug.LogWarning("Punch audio clips not assigned!");
        }
    }

    void SpawnImpactEffect(Collision collision)
    {
        if (impactParticlePrefab != null && collision.contactCount > 0)
        {
            // World impact effect
            ContactPoint contact = collision.GetContact(0);
            GameObject impact = Instantiate(
                impactParticlePrefab,
                contact.point,
                Quaternion.LookRotation(contact.normal)
            );
            Destroy(impact, 2f);

        }
    }

}