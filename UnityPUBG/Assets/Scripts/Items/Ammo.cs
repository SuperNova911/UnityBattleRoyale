using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG Items/Ammo")]
    public class Ammo : UsableItem
    {
        public Ammo(int id, string name, ItemRarity rarity, int maxStack) : base(id, name, rarity, maxStack)
        {
        }
    }
}
