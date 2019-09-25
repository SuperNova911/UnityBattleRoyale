using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Items
{
    public class HealthKit : UsableItem
    {
        public HealthKit(int id, string name, ItemRarity rarity, int maxStack) : base(id, name, rarity, maxStack)
        {
        }
    }
}
