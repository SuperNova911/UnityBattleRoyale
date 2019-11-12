using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Utilities;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts
{
    public class ItemSpawnManager : Singleton<ItemSpawnManager>
    {
        [SerializeField] private ItemObject baseItemObjectPrefab;

        #region 유니티 메시지
        #endregion

        /// <summary>
        /// 매개변수로 받은 위치에 ItemObject를 생성
        /// </summary>
        /// <param name="item">생성할 아이템</param>
        /// <param name="position">ItemObject를 생성할 위치</param>
        /// <returns>생성된 ItemObject</returns>
        public ItemObject SpawnItemObjectAt(Item item, Vector3 position)
        {
            if (item == null || item.IsStackEmpty)
            {
                return null;
            }

            if (baseItemObjectPrefab == null)
            {
                Debug.LogError($"{nameof(baseItemObjectPrefab)}는 null일 수 없습니다");
                return null;
            }

            var instantiatedObject = PhotonNetwork.Instantiate(baseItemObjectPrefab.name, position, Quaternion.identity, 0);
            if (instantiatedObject == null)
            {
                Debug.LogError($"{nameof(PhotonNetwork)}를 통해 {nameof(baseItemObjectPrefab)}를 생성하지 못했습니다");
                return null;
            }

            var baseItemObject = instantiatedObject.GetComponent<ItemObject>();
            baseItemObject.Item = item;
            baseItemObject.NotifyUpdateItem();

            return baseItemObject;
        }

        public void SpawnRandomItemsAtSpawnPoints()
        {
            var targetSpawnPoints = FindAllSpawnPoints();
            foreach (var spawnPoint in targetSpawnPoints)
            {
                var selectedItemData = ItemDataCollection.Instance.SelectRandomItemData(spawnPoint.SpawnChance);
                if (selectedItemData == null)
                {
                    continue;
                }

                SpawnItemObjectAt(selectedItemData.BuildItem(), spawnPoint.transform.position);
            }
        }

        /// <summary>
        /// ItemSpawnPoint Tag와 Component가 있는 GameObject의 ItemSpawnPoint 리스트를 반환
        /// </summary>
        /// <returns>ItemSpawnPoint의 리스트</returns>
        private List<ItemSpawnPoint> FindAllSpawnPoints()
        {
            const string tagName = "ItemSpawnPoint";

            return GameObject
                .FindGameObjectsWithTag(tagName)
                .Where(e => e.GetComponent<ItemSpawnPoint>() != null)
                .Select(e => e.GetComponent<ItemSpawnPoint>())
                .ToList();
        }
    }
}
