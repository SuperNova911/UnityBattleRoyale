using UnityEngine;

public abstract class ConsumableData : ItemData
{
    [Header("Comsumable Settings")]
    [SerializeField, Range(2f, 10f)] private float timeToUse = 4f;

    public float TimeToUse => timeToUse;
}