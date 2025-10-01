using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 25f, -3f);

    void LateUpdate()
    {
        transform.position = player.position + offset;
    }
}
