using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    public class ShieldItem : Item
    {
        private float currentShield;

        public ShieldItem(ArmorData armorData, float initialShield) : base(armorData)
        {
            MaximumShield = armorData.ShieldAmount;
            CurrentShield = initialShield;
        }

        public int MaximumShield { get; private set; }
        public float CurrentShield
        {
            get { return currentShield; }
            set
            {
                float previousShield = currentShield;
                currentShield = Mathf.Clamp(value, 0f, MaximumShield);
            }
        }
    }
}
