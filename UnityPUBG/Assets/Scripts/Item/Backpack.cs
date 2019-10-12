using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG Items/Backpack")]
    public class Backpack : Item
    {
        [SerializeField] private int bonusCapacity = 2;

        public int BonusCapacity => bonusCapacity;
    }
}
