using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG Items/Armor")]
    public class Armor : Item
    {
        public Armor(string name, ItemRarity rarity, int shieldAmount) : base(name, rarity)
        {
            ShieldAmount = shieldAmount;
        }
        
        public int ShieldAmount { get; private set; }
    }
}
