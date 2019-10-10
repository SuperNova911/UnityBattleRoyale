using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    public abstract class Weapon : Item
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackSpeed = 1f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float knockbackPower = 1f;

        public float Damage => damage;
        public float AttackSpeed => attackSpeed;
        public float AttackRange => attackRange;
        public float KnockbackPower => knockbackPower;
    }
}
