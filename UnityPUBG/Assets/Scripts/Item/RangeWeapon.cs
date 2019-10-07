using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG Items/RangeWeapon")]
    public class RangeWeapon : Weapon
    {
        public RangeWeapon(string name, ItemRarity rarity, float damage, float attackSpeed, float attackRange, float knockbackPower) : base(name, rarity, damage, attackSpeed, attackRange, knockbackPower)
        {
        }
    }
}
