using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public abstract class Item
    {
        protected Item(int id, string name, ItemRarity rarity)
        {
            ID = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Rarity = rarity;
        }

        public int ID { get; private set; }
        public string Name { get; private set; }
        public ItemRarity Rarity { get; private set; }
    }
}