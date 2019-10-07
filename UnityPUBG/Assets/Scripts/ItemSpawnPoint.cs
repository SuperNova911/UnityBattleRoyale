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
    public sealed class ItemSpawnPoint : MonoBehaviour
    {
        #region 필드
        public ItemSpawnRate spawnRate = new ItemSpawnRate();
        [SerializeField] [ReadOnly] private ItemSpawnGroup spawnGroup;
        [SerializeField] [ReadOnly] private ItemObject spawnedItem;
        #endregion

        #region 속성
        public ItemObject SpawnedItem
        {
            get { return spawnedItem; }
            set
            {
                spawnedItem = value;
                spawnedItem.transform.parent = transform;
                spawnedItem.transform.localPosition = Vector3.zero;
            }
        }
        #endregion

        #region 유니티 메시지
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
        #endregion
    }
}
