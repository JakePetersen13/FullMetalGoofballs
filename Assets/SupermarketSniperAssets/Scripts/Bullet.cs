using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToLive;
    [SerializeField] TrailRenderer trail;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        if (trail == null)
        {
            trail = GetComponentInChildren<TrailRenderer>();
        }
    }

    void OnEnable()
    {
        if (trail) trail.Clear();
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToLive) DestroyBullet();
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroyBullet();
    }

    void DestroyBullet()
    {
        if (trail != null)
        {
            trail.transform.parent = null;
            trail.autodestruct = true;
        }

        Destroy(gameObject);
    }
}
