using UnityEngine;

[CreateAssetMenu(menuName = "UnityPUBG/ItemData/HealingKit")]
public class HealingKitData : ConsumableData
{
    [Header("Healing Kit Settings")]
    [SerializeField, Range(0f, 100f)] private float healthRestoreAmount = 25f;
    [SerializeField, Range(0f, 100f)] private float shieldRestoreAmount = 0f;

    public float HealthRestoreAmount => healthRestoreAmount;
    public float ShieldRestoreAmount => shieldRestoreAmount;
}