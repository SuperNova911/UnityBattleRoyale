using UnityEngine;

[CreateAssetMenu(menuName = "UnityPUBG/ItemData/RangeWeapon")]
public class RangeWeaponData : WeaponData
{
    [Header("Range Weapon Settings")]
    [SerializeField] private AmmoData requireAmmo = null;
    [SerializeField] private float movementSpeedMultiplier = 0.5f;

    public AmmoData RequireAmmo => requireAmmo;
    public float MovementSpeedMultiplier => movementSpeedMultiplier;
}