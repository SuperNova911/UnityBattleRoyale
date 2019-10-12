using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;

namespace UnityPUBG.Scripts
{
    /// <summary>
    /// 아이템 스폰 확률 정보를 관리하는 클래스
    /// </summary>
    [Serializable]
    public class ItemSpawnChance
    {
        #region 필드
        [Header("Spawn chance")]
        [SerializeField] [Range(0f, 1f)] private float spawnChance = 0.75f;

        [Header("Rarity ratio")]
        [SerializeField] [Range(0f, 1f)] private float commonRatio = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float rareRatio = 0.3f;
        [SerializeField] [Range(0f, 1f)] private float epicRatio = 0.1f;
        [SerializeField] [Range(0f, 1f)] private float legendaryRatio = 0.05f;
        #endregion

        #region 생성자
        /// <summary>
        /// 새로운 ItemSpawnChance 생성, 기본값 사용
        /// </summary>
        public ItemSpawnChance()
        {

        }

        /// <summary>
        /// 새로운 ItemSpawnChance 생성, 0과 1사이의 값 입력
        /// </summary>
        /// <param name="spawnChance">아이템 스폰 확률</param>
        /// <param name="commonRatio">Common 등급 아이템 비율</param>
        /// <param name="rareRatio">Rare 등급 아이템 비율</param>
        /// <param name="epicRatio">Epic 등급 아이템 비율</param>
        /// <param name="legendaryRatio">Legendary 등급 아이템 비율</param>
        public ItemSpawnChance(float spawnChance, float commonRatio, float rareRatio, float epicRatio, float legendaryRatio)
        {
            this.spawnChance = Mathf.Clamp01(spawnChance);
            this.commonRatio = Mathf.Clamp01(commonRatio);
            this.rareRatio = Mathf.Clamp01(rareRatio);
            this.epicRatio = Mathf.Clamp01(epicRatio);
            this.legendaryRatio = Mathf.Clamp01(legendaryRatio);
        }
        #endregion

        #region 속성
        /// <summary>
        /// 아이템 스폰 확률
        /// </summary>
        public float SpawnChance => spawnChance;

        /// <summary>
        /// Common 등급 아이템 비율
        /// </summary>
        public float CommonRatio => commonRatio;
        /// <summary>
        /// Rare 등급 아이템 비율
        /// </summary>
        public float RareRatio => rareRatio;
        /// <summary>
        /// Epic 등급 아이템 비율
        /// </summary>
        public float EpicRatio => epicRatio;
        /// <summary>
        /// Legendary 등급 아이템 비율
        /// </summary>
        public float LegendaryRatio => legendaryRatio;
        #endregion

        /// <summary>
        /// 각 등급별 비율을 기반으로 무작위로 등급 하나를 선택
        /// </summary>
        /// <returns>무작위 아이템 등급</returns>
        public ItemRarity GetRandomItemRarity()
        {
            float[] rarityRatios = new float[] { CommonRatio, RareRatio, EpicRatio, LegendaryRatio };
            float random = UnityEngine.Random.Range(0f, rarityRatios.Sum(e => e));

            for (int index = 0; index < rarityRatios.Length; index++)
            {
                random -= rarityRatios[index];
                if (random <= 0)
                {
                    return (ItemRarity)index;
                }
            }

            return ItemRarity.Legendary;
        }
    }
}
