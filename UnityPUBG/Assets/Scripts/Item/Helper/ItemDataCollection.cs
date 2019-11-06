using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/ItemCollection")]
    public class ItemDataCollection : ScriptableObject
    {
        [SerializeField] private List<ItemData> itemDataCollection = new List<ItemData>();

        #region 유니티 메시지
        private void OnEnable()
        {
            InitializeCollection();
        }
        #endregion

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
