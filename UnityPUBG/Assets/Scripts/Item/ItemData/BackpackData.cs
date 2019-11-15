using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/ItemData/Backpack")]
    public class BackpackData : ItemData
    {
        [SerializeField, Range(0, 6)] private int bonusCapacity = 2;

        public int BonusCapacity => bonusCapacity;
    }
}
