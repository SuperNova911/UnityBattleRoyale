using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/ItemData/EmptyItem")]
    public sealed class EmptyItemData : ItemData
    {
        public override int MaximumStack => 0;
        public override int DefaultStack => 0;
    }
}
