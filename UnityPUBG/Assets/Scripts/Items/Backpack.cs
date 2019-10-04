using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG Items/Backpack")]
    public class Backpack : Item
    {
        [SerializeField] private int bonusCapacity;

        public Backpack(int id, string name, ItemRarity rarity, int bonusCapacity) : base(id, name, rarity)
        {
            this.bonusCapacity = bonusCapacity;
        }

        public int BonusCapacity => bonusCapacity;
    }
}
