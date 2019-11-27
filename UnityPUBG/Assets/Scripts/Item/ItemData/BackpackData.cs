using UnityEngine;

[CreateAssetMenu(menuName = "UnityPUBG/ItemData/Backpack")]
public class BackpackData : ItemData
{
    [Header("Backpack Settings")]
    [SerializeField, Range(0, 6)] private int bonusCapacity = 2;

    public int BonusCapacity => bonusCapacity;
}