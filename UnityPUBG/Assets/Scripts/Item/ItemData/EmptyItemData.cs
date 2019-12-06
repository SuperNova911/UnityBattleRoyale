using UnityEngine;

[CreateAssetMenu(menuName = "UnityPUBG/ItemData/EmptyItem")]
public sealed class EmptyItemData : ItemData
{
    public override int MaximumStack => 0;
    public override int DefaultStack => 0;
}