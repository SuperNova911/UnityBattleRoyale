using UnityEngine;
using UnityPUBG.Scripts.Utilities;

public abstract class WeaponData : ItemData
{
    [Header("Weapon Settings")]
    [SerializeField, Range(0f, 200f)] private float damage = 10f;
    [SerializeField] private DamageType damageType = DamageType.Normal;
    [SerializeField, Range(0.1f, 5f)] private float castDelay = 0.1f;
    [SerializeField, Range(0.1f, 5f)] private float attackCooldown = 1f;
    [SerializeField, Range(0.1f, 30f)] private float attackRange = 2f;
    [SerializeField, Range(0f, 10f)] private float knockbackPower = 1f;

    public float Damage => damage;
    public DamageType DamageType => damageType;
    public float CastDelay => castDelay;
    public float AttackCooldown => attackCooldown;
    public float AttackRange => attackRange;
    public float KnockbackPower => knockbackPower;
}