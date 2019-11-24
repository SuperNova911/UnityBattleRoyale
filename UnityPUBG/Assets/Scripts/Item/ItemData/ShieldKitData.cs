using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/ItemData/ShieldKit")]
    public class ShieldKitData : ConsumableItemData
    {
        [Header("ShieldKit Settings")]
        [SerializeField, Range(10f, 100f)] private float healAmout = 25f;

        public float HealAmount => healAmout;
    }
}
