using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    public abstract class ConsumableItemData : ItemData
    {
        [Header("Comsumable Settings")]
        [SerializeField, Range(2f, 10f)] private float timeToUse = 4f;

        public float TimeToUse => timeToUse;
    }
}