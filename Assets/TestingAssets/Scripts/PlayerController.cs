using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("---Physics---")]
    public float maxSpeed = 15f;
    public float accelerationRate = 5f;
    public float gravityForce = 15f;
    [Header("---References---")]
    public Rigidbody rb;
    public Camera mainCamera;

    void FixedUpdate()
    {
        HandleMovement();
        RotateTowardsMouse();
    }

    void HandleMovement()
    {
        //-----MOVEMENT-----
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (moveDir.sqrMagnitude > 0.01f)
        {
            // Applies force based on moveDir vector
            moveDir.Normalize();
            rb.AddForce(moveDir * accelerationRate, ForceMode.Acceleration);

            // maxSpeed cap
            if (rb.velocity.magnitude > maxSpeed)
                rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else
        {
            // deceleration
            if (rb.velocity.magnitude > 0.01f)
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
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 0.2f));
            }
        }
    }
}
