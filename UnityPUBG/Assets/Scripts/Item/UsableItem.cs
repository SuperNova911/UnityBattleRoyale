using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityPUBG.Scripts.Items
{
    public abstract class UsableItem : Item
    {
        public UsableItem(int id, string name, ItemRarity rarity, int maxStack) : base(id, name, rarity)
        {
            MaximumStack = maxStack;
            CurrentStack = 1;
        }

        public int MaximumStack { get; private set; }
        public int CurrentStack { get; private set; }
    }
}
