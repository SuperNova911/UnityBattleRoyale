using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Logic
{
    /// <summary>
    /// 아이템 스폰 지점
    /// </summary>
    public sealed class ItemSpawnPoint : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private ItemSpawnChance spawnChance = new ItemSpawnChance();
        [SerializeField] private ItemSpawnGroup spawnGroup;
        [SerializeField] private bool useGroupSetting = true;

        [Header("Gizmo Settings")]
        [SerializeField] private Color color = Color.white;
        [SerializeField] private bool showGizmo = true;

        /// <summary>
        /// 스폰 지점이 속해있는 그룹
        /// </summary>
        public ItemSpawnGroup SpawnGroup
        {
            get { return spawnGroup; }
            set { spawnGroup = value; }
        }
        /// <summary>
        /// 스폰 지점의 아이템 스폰 확률 정보
        /// </summary>
        public ItemSpawnChance SpawnChance
        {
            get { return useGroupSetting && spawnGroup != null ? spawnGroup.SpawnChance : spawnChance; }
        }

        #region 유니티 메시지
        private void OnDrawGizmos()
        {
            if (showGizmo)
            {
                Gizmos.color = useGroupSetting && spawnGroup != null ? spawnGroup.AreaColor : color;
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
        #endregion
    }
}
