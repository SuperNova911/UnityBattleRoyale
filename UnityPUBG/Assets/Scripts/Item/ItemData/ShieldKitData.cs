using UnityEngine;

[CreateAssetMenu(menuName = "UnityPUBG/ItemData/ShieldKit")]
public class ShieldKitData : ConsumableData
{
    [Header("Shield Kit Settings")]
    [SerializeField, Range(10f, 100f)] private float healAmout = 25f;

    public float HealAmount => healAmout;
}