using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 15f;
    public float moveSpeed = 5f;

    [Header(" ")]
    public Rigidbody rb;

    void FixedUpdate()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        if (moveDir.sqrMagnitude > 0.01f)
        {
            moveDir.Normalize();
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.Acceleration);

            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
        else
        {
            if (rb.velocity.magnitude > 0.01f)
            {
                Vector3 decelDir = -rb.velocity.normalized;
                rb.AddForce(decelDir * moveSpeed, ForceMode.Acceleration);

                if (rb.velocity.magnitude < 0.1f)
                    rb.velocity = Vector3.zero;
            }
        }


    }
}
