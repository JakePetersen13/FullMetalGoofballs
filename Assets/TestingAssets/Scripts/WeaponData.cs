using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("---Weapon Stats---")]
    public string weaponName = "Sword";
    public float damage = 25f;
    public float speedMultiplier = 1f; // 1 = normal, 0.8 = slower, 1.2 = faster
    public float lungeForceMultiplier = 1f; // Affects lunge distance

    [Header("---Visual---")]
    public GameObject weaponPrefab; // The actual weapon model

    [Header("---Weapon Size---")]
    [Tooltip("Small weapons are fast, large weapons are slow but powerful")]
    public WeaponSize size = WeaponSize.Medium;
}

public enum WeaponSize
{
    Small,   // Fast, low damage (Dagger, Knife)
    Medium,  // Balanced (Sword, Mace)
    Large    // Slow, high damage (Greatsword, Hammer)
}