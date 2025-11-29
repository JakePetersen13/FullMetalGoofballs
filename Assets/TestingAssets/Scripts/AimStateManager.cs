using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AimStateManager : MonoBehaviour
{
    // Look control variables
    float xAxis, yAxis;
    [SerializeField] Transform camFollowPos;
    [SerializeField] bool invertY = false;
    [SerializeField] float mouseSense = 1f;

    // Aiming variables
    [SerializeField] Transform aimPos;
    [SerializeField] float aimSmoothSpeed = 20f;
    [SerializeField] float maxAimRadius = 10f;
    [SerializeField] Transform aimCenter; // If null, uses camFollowPos
    [SerializeField] LayerMask aimMask;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        xAxis += Input.GetAxisRaw("Mouse X") * mouseSense;
        yAxis += Input.GetAxisRaw("Mouse Y") * mouseSense;
        yAxis = Mathf.Clamp(yAxis, -80f, 80f);
    }

    private void LateUpdate()
    {
        float appliedY = invertY ? -yAxis : yAxis;
        camFollowPos.localEulerAngles = new Vector3(appliedY, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.SphereCast(ray, 0.1f, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            Vector3 center = (aimCenter != null ? aimCenter.position : camFollowPos.position);
            Vector3 targetPoint = hit.point;
            Vector3 clampedPoint = center + Vector3.ClampMagnitude(targetPoint - center, maxAimRadius);
            aimPos.position = Vector3.Lerp(aimPos.position, clampedPoint, aimSmoothSpeed * Time.deltaTime);
        }
    }
}
