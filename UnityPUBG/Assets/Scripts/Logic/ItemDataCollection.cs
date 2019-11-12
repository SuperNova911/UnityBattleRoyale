using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;

namespace UnityPUBG.Scripts.Logic
{
    public class ItemDataCollection : Singleton<ItemDataCollection>
    {
        [SerializeField] private List<ItemData> itemDataCollection = new List<ItemData>();

        /// <summary>
        /// 등록된 모든 아이템들이 있는 ReadOnlyCollection
        /// </summary>
        public ReadOnlyCollection<ItemData> ItemDatas { get; private set; }
        /// <summary>
        /// 아이템의 이름으로 접근 가능한 ReadOnlyDictionary
        /// </summary>
        public ReadOnlyDictionary<string, ItemData> ItemDataByName { get; private set; }

        /// <summary>
        /// 아이템들을 아이템 타입으로 접근 가능한 ReadOnlyDictionary
        /// </summary>
        public ReadOnlyDictionary<Type, ReadOnlyCollection<ItemData>> ItemDatasByType { get; private set; }
        /// <summary>
        /// 아이템들을 아이템 등급으로 접근 가능한 ReadOnlyDictionary
        /// </summary>
        public ReadOnlyDictionary<ItemRarity, ReadOnlyCollection<ItemData>> ItemDatasByRarity { get; private set; }

        #region 유니티 메시지
        private void Awake()
        {
            InitializeCollection();
        }
        #endregion

        /// <summary>
        /// 매개변수로 받은 ItemSpawnChance 기반으로 무작위 아이템 데이터를 선택
        /// </summary>
        /// <param name="spawnChance">스폰 확률 정보</param>
        /// <returns>무작위로 선택된 아이템 데이터</returns>
        public ItemData SelectRandomItemData(ItemSpawnChance spawnChance)
        {
            if (UnityEngine.Random.value <= spawnChance.SpawnChance)
            {
                ItemRarity selectedRarity = spawnChance.GetRandomItemRarity();
                if (ItemDatasByRarity.TryGetValue(selectedRarity, out var itemDatas))
                {
                    int randomIndex = UnityEngine.Random.Range(0, itemDatas.Count);
                    return itemDatas[randomIndex];
                }
                else
                {
                    Debug.LogWarning($"{nameof(ItemRarity)}: {selectedRarity}에 해당하는 아이템 데이터 컬렉션이 없습니다");
                    return null;
                }
            }
            else
            {
                // 아이템을 스폰하지 않음
                return null;
            }
        }

        /// <summary>
        /// itemCollection을 기반으로 접근 가능한 다양한 Collection들을 초기화
        /// </summary>
        private void InitializeCollection()
        {
            var itemDatas = new List<ItemData>();
            var itemDataByName = new Dictionary<string, ItemData>();
            var itemDatasByType = new Dictionary<Type, IList<ItemData>>();
            var itemDatasByRarity = new Dictionary<ItemRarity, IList<ItemData>>();

            foreach (ItemData data in itemDataCollection)
            {
                if (data == null)
                {
                    Debug.LogError("itemCollection에 null값이 포함되어 있습니다");
                    continue;
                }

                if (itemDataByName.ContainsKey(data.ItemName))
                {
                    Debug.LogError($"중복된 이름을 가진 아이템 데이터가 있습니다, {nameof(data.ItemName)}: {data.ItemName}");
                    continue;
                }

                // All ItemDatas
                itemDatas.Add(data);

                // ItemData By Name
                itemDataByName.Add(data.ItemName, data);

                // ItemDatas By Type
                if (itemDatasByType.TryGetValue(data.GetType(), out var dataList))
                {
                    dataList.Add(data);
                }
                else
                {
                    itemDatasByType.Add(data.GetType(), new List<ItemData> { { data } });
                }

                // ItemDatas By Rarity
                if (itemDatasByRarity.TryGetValue(data.Rarity, out dataList))
                {
                    dataList.Add(data);
                }
                else
                {
                    itemDatasByRarity.Add(data.Rarity, new List<ItemData> { { data } });
                }
            }

            ItemDatas = itemDatas.AsReadOnly();
            ItemDataByName = new ReadOnlyDictionary<string, ItemData>(itemDataByName);
            ItemDatasByType = new ReadOnlyDictionary<Type, ReadOnlyCollection<ItemData>>(itemDatasByType
                .ToDictionary(k => k.Key, v => ((List<ItemData>)v.Value).AsReadOnly()));
            ItemDatasByRarity = new ReadOnlyDictionary<ItemRarity, ReadOnlyCollection<ItemData>>(itemDatasByRarity
                .ToDictionary(k => k.Key, v => ((List<ItemData>)v.Value).AsReadOnly()));
        } 
    }
}
