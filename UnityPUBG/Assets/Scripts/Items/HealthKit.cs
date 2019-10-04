using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG Items/HealthKit")]
    public class HealthKit : UsableItem
    {
        public HealthKit(int id, string name, ItemRarity rarity, int maxStack) : base(id, name, rarity, maxStack)
        {
        }
    }
}
