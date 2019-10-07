using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG Items/Ammo")]
    public class Ammo : UsableItem
    {
        public Ammo(string name, ItemRarity rarity, int maxStack) : base(name, rarity, maxStack)
        {
        }
    }
}
