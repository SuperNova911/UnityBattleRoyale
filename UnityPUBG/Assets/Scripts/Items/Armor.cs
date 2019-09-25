using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Items
{
    public class Armor : Item
    {
        public Armor(int id, string name, ItemRarity rarity, int shieldAmount) : base(id, name, rarity)
        {
            ShieldAmount = shieldAmount;
        }
        
        public int ShieldAmount { get; private set; }
    }
}
