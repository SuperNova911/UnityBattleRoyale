using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] private string itemName = string.Empty;
        [SerializeField] private ItemRarity rarity = ItemRarity.Common;
        [SerializeField] private GameObject model = null;

        public string ItemName => itemName;
        public ItemRarity Rarity => rarity;
        public GameObject Model => model;
    }
}