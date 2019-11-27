using UnityEngine;

[CreateAssetMenu(menuName = "UnityPUBG/ItemData/Armor")]
public class ArmorData : ItemData
{
    [Header("Armor Settings")]
    [SerializeField, Range(0, 100)] private int shieldAmount = 50;

    public int ShieldAmount => shieldAmount;
}