using UnityEngine;

public class DebugHandAim : MonoBehaviour
{
    public Transform aimTarget;  // drag your AimPosition sphere here

    void LateUpdate()
    {
        if (!aimTarget) return;

        // Make this pivot look exactly at the target
        Vector3 dir = aimTarget.position - transform.position;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }
}
