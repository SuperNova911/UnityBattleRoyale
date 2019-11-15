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
        // TODO: EmptyItemData 인스턴스를 만들지 않아도 작동하는지 테스트
        public EmptyItem() : base(ScriptableObject.CreateInstance<EmptyItemData>())
        {
        }
    }
}