using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG Items/ItemCollection")]
    public class ItemCollection : ScriptableObject
    {
        #region 필드
        [SerializeField] private List<Item> itemCollection = new List<Item>();
        #endregion

        #region 유니티 메시지
        private void Awake()
        {
            InitializeCollection();
        }
        #endregion

        #region 속성
        /// <summary>
        /// 등록된 모든 아이템들이 있는 ReadOnlyCollection
        /// </summary>
        public ReadOnlyCollection<Item> Items { get; private set; }
        /// <summary>
        /// 아이템의 이름으로 접근 가능한 ReadOnlyDictionary
        /// </summary>
        public ReadOnlyDictionary<string, Item> ItemByName { get; private set; }

        /// <summary>
        /// 아이템들을 아이템 타입으로 접근 가능한 ReadOnlyDictionary
        /// </summary>
        public ReadOnlyDictionary<Type, ReadOnlyCollection<Item>> ItemsByType { get; private set; }
        /// <summary>
        /// 아이템들을 아이템 등급으로 접근 가능한 ReadOnlyDictionary
        /// </summary>
        public ReadOnlyDictionary<ItemRarity, ReadOnlyCollection<Item>> ItemsByRarity { get; private set; }
        #endregion

        #region 메서드
        /// <summary>
        /// itemCollection을 기반으로 접근 가능한 다양한 Collection들을 초기화
        /// </summary>
        private void InitializeCollection()
        {
            var items = new List<Item>();
            var itemByName = new Dictionary<string, Item>();
            var itemsByType = new Dictionary<Type, IList<Item>>();
            var itemsByRarity = new Dictionary<ItemRarity, IList<Item>>();

            foreach (Item item in itemCollection)
            {
                if (item == null)
                {
                    Debug.LogError("itemCollection에 null값이 포함되어 있습니다");
                    continue;
                }

                if (itemByName.ContainsKey(item.ItemName))
                {
                    Debug.LogError($"중복된 이름을 가진 Item이 있습니다, {nameof(item.ItemName)}: {item.ItemName}");
                    continue;
                }

                // All Items
                items.Add(item);

                // Item By Name
                itemByName.Add(item.ItemName, item);

                // Items By Type
                if (itemsByType.TryGetValue(item.GetType(), out var itemList))
                {
                    itemList.Add(item);
                }
                else
                {
                    itemsByType.Add(item.GetType(), new List<Item> { { item } });
                }

                // Items By Rarity
                if (itemsByRarity.TryGetValue(item.Rarity, out itemList))
                {
                    itemList.Add(item);
                }
                else
                {
                    itemsByRarity.Add(item.Rarity, new List<Item> { { item } });
                }
            }

            Items = items.AsReadOnly();
            ItemByName = new ReadOnlyDictionary<string, Item>(itemByName);
            ItemsByType = new ReadOnlyDictionary<Type, ReadOnlyCollection<Item>>(itemsByType
                .ToDictionary(k => k.Key, v => ((List<Item>)v.Value).AsReadOnly()));
            ItemsByRarity = new ReadOnlyDictionary<ItemRarity, ReadOnlyCollection<Item>>(itemsByRarity
                .ToDictionary(k => k.Key, v => ((List<Item>)v.Value).AsReadOnly()));
        } 
        #endregion
    }
}
