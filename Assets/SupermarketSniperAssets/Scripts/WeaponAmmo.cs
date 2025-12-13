using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    public float clipSize;
    public float extraAmmo;
    public float currentAmmo;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = clipSize;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))  Reload();
    }

    public void Reload()
    {
        if (extraAmmo >= clipSize)
        {
            int ammoToReload = (int)(clipSize - currentAmmo);
            extraAmmo -= ammoToReload;
            currentAmmo += ammoToReload;
        }
        else if (extraAmmo > 0)
        {
            int leftOverAmmo = (int)(extraAmmo + currentAmmo - clipSize);
            extraAmmo = leftOverAmmo;
            currentAmmo = clipSize;
        }
        else
        {
            currentAmmo += extraAmmo;
            extraAmmo = 0;
        }

    }
}
