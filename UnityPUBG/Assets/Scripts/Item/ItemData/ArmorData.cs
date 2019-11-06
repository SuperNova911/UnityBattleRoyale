using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/ItemData/Armor")]
    public class ArmorData : ItemData
    {
        [SerializeField, Range(0, 100)] private int shieldAmount = 50;

        public int ShieldAmount => shieldAmount;

        public override Item BuildItem()
        {
            return new ItemArmor(this);
        }
    }
}
