using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    public int clipSize;
    public int extraAmmo;
    public int currentAmmo;

    public AudioClip reloadSound;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = clipSize;
    }

    public void Reload()
    {
        if (extraAmmo >= clipSize)
        {
            int ammoToReload = clipSize - currentAmmo;
            extraAmmo -= ammoToReload;
            currentAmmo += ammoToReload;
        }
        else if (extraAmmo > 0)
        {
            int leftOverAmmo = extraAmmo + currentAmmo - clipSize;
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
