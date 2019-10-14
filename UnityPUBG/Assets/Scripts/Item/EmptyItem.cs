using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    public sealed class EmptyItem : Item
    {
        public EmptyItem() : base(ScriptableObject.CreateInstance<EmptyItemData>())
        {
        }

        public override object Clone()
        {
            return this;
        }
    }
}