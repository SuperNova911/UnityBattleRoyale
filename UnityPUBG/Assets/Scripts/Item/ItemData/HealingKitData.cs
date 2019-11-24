using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/ItemData/HealingKit")]
    public class HealingKitData : ConsumableItemData
    {
        [Header("HealingKit Settings")]
        [SerializeField, Range(0f, 100f)] private float healthRestoreAmount = 25f;
        [SerializeField, Range(0f, 100f)] private float shieldRestoreAmount = 0f;

        public float HealthRestoreAmount => healthRestoreAmount;
        public float ShieldRestoreAmount => shieldRestoreAmount;
    }
}
