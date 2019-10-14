using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    public abstract class ItemData : ScriptableObject
    {
        #region 필드
        [SerializeField] private string itemName = string.Empty;
        [SerializeField] private ItemRarity rarity = ItemRarity.Common;
        [SerializeField] private ItemSortingOrder sortingOrder = ItemSortingOrder.Item;
        [SerializeField] private GameObject model = null;
        [SerializeField, Range(1, 100)] private int maximumStack = 1;
        [SerializeField, Range(1, 100)] private int defaultStack = 1;
        #endregion

        #region 속성
        public string ItemName => itemName;
        public ItemRarity Rarity => rarity;
        public GameObject Model => model;
        public virtual ItemSortingOrder SortingOrder => sortingOrder;
        public virtual int MaximumStack => maximumStack;
        public virtual int DefaultStack => defaultStack;
        #endregion

        #region 유니티 메시지
        private void OnValidate()
        {
            if (defaultStack > maximumStack)
            {
                defaultStack = maximumStack;
            }
        }
        #endregion

        #region 메서드
        public abstract Item BuildItem();
        #endregion
    }
}