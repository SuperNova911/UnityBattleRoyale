using Assets.Scripts.Entities;
using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public sealed class ItemSpawnPoint : MonoBehaviour
    {
        public ItemSpawnZone parentZone;
        public ItemObject spawnedItem;
        public Item test;

        private void Awake()
        {
        }

        public void SpawnItem()
        {
            spawnedItem = ItemSpawner.Instance.Kappa(test);
            spawnedItem.transform.parent = transform;
            spawnedItem.transform.localPosition = Vector3.zero;
        }
    }
}
