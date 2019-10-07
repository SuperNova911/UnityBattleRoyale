using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts
{
    public class ItemSpawnManager : Singleton<ItemSpawnManager>
    {
        #region 필드
        [SerializeField] private ItemObject baseItemObject;
        [SerializeField] private ItemCollection itemCollection;

        [SerializeField] [ReadOnly] private List<ItemSpawnPoint> spawnPoints;
        #endregion

        #region 유니티 메시지
        private void Awake()
        {
            itemCollection = Instantiate(itemCollection);

            spawnPoints = GameObject
                .FindGameObjectsWithTag("ItemSpawnPoint")
                .Where(e => e.GetComponent<ItemSpawnPoint>() != null)
                .Select(e => e.GetComponent<ItemSpawnPoint>())
                .ToList();
        }

        private void Start()
        {
            SpawnItems();
        }
        #endregion

        #region 메서드
        public ItemObject SpawnItemObject(Item item)
        {
            if (item.Model == null)
            {
                // Log
            }

            ItemObject itemObject = Instantiate(baseItemObject);

            itemObject.item = item;
            itemObject.itemModel = Instantiate(item.Model, itemObject.transform);

            return itemObject;
        }

        public Item GetRandomItem()
        {
            int random = UnityEngine.Random.Range(0, itemCollection.Items.Count);
            var item = itemCollection.Items[random];
            return item;
        }

        private void SpawnItems()
        {
            foreach (var spawnPoint in spawnPoints)
            {
                Item randomItem = GetRandomItem();
                ItemObject itemObject = SpawnItemObject(randomItem);
                spawnPoint.SpawnedItem = itemObject;
            }
        }
        #endregion
    }
}
