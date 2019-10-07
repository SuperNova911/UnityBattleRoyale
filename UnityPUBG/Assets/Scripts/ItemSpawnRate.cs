using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;

namespace UnityPUBG.Scripts
{
    [Serializable]
    public class ItemSpawnRate
    {
        #region 필드
        [Header("Spawn chance by Rarity")]
        [SerializeField] [Range(0f, 1f)] private float common = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float rare = 0.3f;
        [SerializeField] [Range(0f, 1f)] private float epic = 0.1f;
        [SerializeField] [Range(0f, 1f)] private float legendary = 0.05f;
        private Func<Item, bool> customFilter;
        #endregion

        #region 속성
        public float Common => common;
        public float Rare => rare;
        public float Epic => epic;
        public float Legendary => legendary;
        #endregion
    }
}
