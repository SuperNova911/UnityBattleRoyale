using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityPUBG.Scripts.Items
{
    public class ItemAmmo : Item
    {
        public ItemAmmo(ItemData data) : base(data)
        {

        }

        public override object Clone()
        {
            var cloneItem = new ItemAmmo(Data);
            cloneItem.CurrentStack = CurrentStack;

            return cloneItem;
        }
    }
}
