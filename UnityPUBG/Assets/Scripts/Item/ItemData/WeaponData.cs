using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    public abstract class WeaponData : ItemData
    {
        [SerializeField, Range(0f, 200f)] private float damage = 10f;
        [SerializeField, Range(0.1f, 5f)] private float attackSpeed = 1f;
        [SerializeField, Range(0.1f, 5f)] private float attackRange = 1f;
        [SerializeField, Range(0f, 10f)] private float knockbackPower = 1f;

        public float Damage => damage;
        public float AttackSpeed => attackSpeed;
        public float AttackRange => attackRange;
        public float KnockbackPower => knockbackPower;
    }
}
