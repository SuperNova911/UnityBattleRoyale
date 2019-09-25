using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Items
{
    public class Backpack : Item
    {
        public Backpack(int id, string name, ItemRarity rarity, int bonusCapacity) : base(id, name, rarity)
        {
            BonusCapacity = bonusCapacity;
        }

        public int BonusCapacity { get; private set; }
    }
}
