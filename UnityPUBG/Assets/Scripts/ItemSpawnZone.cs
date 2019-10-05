using UnityPUBG.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts
{
    public sealed class ItemSpawnZone : MonoBehaviour
    {
        public ItemRarity zoneRarity = ItemRarity.Common;
        public List<ItemSpawnPoint> spawnPoints = new List<ItemSpawnPoint>();

        private void Awake()
        {
            spawnPoints.Clear();

            foreach (var spawnPoint in GetComponentsInChildren<ItemSpawnPoint>())
            {
                spawnPoint.parentZone = this;
                spawnPoints.Add(spawnPoint);
            }
        }

        private void Start()
        {
            foreach (var spawnPoint in spawnPoints)
            {
                spawnPoint.SpawnItem();
            }
        }
    }
}
