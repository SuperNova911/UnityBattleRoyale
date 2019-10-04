using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] private int id;
        [SerializeField] private string itemName;
        [SerializeField] private ItemRarity rarity;
        [SerializeField] private GameObject model;

        protected Item(int id, string itemName, ItemRarity rarity)
        {
            this.id = id;
            this.itemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            this.rarity = rarity;
        }

        public int Id => id;
        public string ItemName => itemName;
        public ItemRarity Rarity => rarity;
        public GameObject Model => model;
    }
}