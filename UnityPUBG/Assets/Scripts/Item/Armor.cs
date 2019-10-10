using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG Items/Armor")]
    public class Armor : Item
    {
        [SerializeField] private int shieldAmount = 50;

        public int ShieldAmount => shieldAmount;
    }
}
