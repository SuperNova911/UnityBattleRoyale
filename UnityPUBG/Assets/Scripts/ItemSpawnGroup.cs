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
    public sealed class ItemSpawnGroup : MonoBehaviour
    {
        #region 필드
        public ItemSpawnRate groupSpawnRate = new ItemSpawnRate();
        [SerializeField] [ReadOnly] private List<ItemSpawnPoint> spawnPoints = new List<ItemSpawnPoint>();
        #endregion

        #region 유니티 메시지
        private void Awake()
        {
            spawnPoints.Clear();

            foreach (var spawnPoint in GetComponentsInChildren<ItemSpawnPoint>())
            {
                spawnPoints.Add(spawnPoint);
            }
        }
        #endregion
    }
}
