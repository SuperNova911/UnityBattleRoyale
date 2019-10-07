using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG Items/Backpack")]
    public class Backpack : Item
    {
        [SerializeField] private int bonusCapacity;

        public Backpack(string name, ItemRarity rarity, int bonusCapacity) : base(name, rarity)
        {
            this.bonusCapacity = bonusCapacity;
        }

        public int BonusCapacity => bonusCapacity;
    }
}
