using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityPUBG.Scripts.Items
{
    public sealed class EmptyItemData : ItemData
    {
        public override int MaximumStack => 0;
        public override int DefaultStack => 0;

        public override Item BuildItem()
        {
            return Item.EmptyItem;
        }
    }
}
