using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts
{
    public class ItemSpawner : Singleton<ItemSpawner>
    {
        [SerializeField] private Material CommonMaterial;
        [SerializeField] private Material RareMaterial;
        [SerializeField] private Material EpicMaterial;
        [SerializeField] private Material LegendaryMaterial;

        [SerializeField] private ItemObject baseItemObject;

        private Dictionary<ItemRarity, Material> itemMaterials;

        private void Awake()
        {
            itemMaterials = new Dictionary<ItemRarity, Material>
            {
                { ItemRarity.Common, CommonMaterial },
                { ItemRarity.Rare, RareMaterial },
                { ItemRarity.Epic, EpicMaterial },
                { ItemRarity.Legendary, LegendaryMaterial },
            };
        }

        public ItemObject Kappa(Item item)
        {
            ItemObject itemObject = Instantiate(baseItemObject);
            itemObject.item = item;
            itemObject.name = item.ItemName;

            itemObject.itemModel = Instantiate(item.Model, itemObject.transform);
            var modelMeshRenderer = itemObject.itemModel.GetComponent<MeshRenderer>();
            if (itemMaterials.TryGetValue(item.Rarity, out Material material))
            {
                modelMeshRenderer.material = material;
            }
            else
            {
                Debug.LogWarning(string.Format("Cannot find match material with rarity, ItemRarity: '{0}'", item.Rarity));
            }

            return itemObject;
        }
    }
}
