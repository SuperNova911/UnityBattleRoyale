using UnityEngine;

[CreateAssetMenu(menuName = "UnityPUBG/ItemData/Ammo")]
public class AmmoData : ItemData
{
    [Header("Ammo Settings")]
    [SerializeField, Range(1f, 100f)] private float projectileSpeed;
    [SerializeField, Range(0.01f, 1f)] private float colliderRadius;

    public float ProjectileSpeed => projectileSpeed;
    public float ColliderRadius => colliderRadius;
}
