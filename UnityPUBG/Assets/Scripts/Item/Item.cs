using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    public abstract class Item : ScriptableObject, IComparable<Item>
    {
        #region 필드
        [SerializeField] private string itemName = string.Empty;
        [SerializeField] private ItemRarity rarity = ItemRarity.Common;
        [SerializeField] private ItemSortingOrder sortingOrder = ItemSortingOrder.Item;
        [SerializeField] private GameObject model = null;
        [SerializeField] [Range(1, 100)] private int maximumStack = 1;
        [SerializeField] [Range(1, 100)] protected int currentStack = 1;
        #endregion

        #region 속성
        public string ItemName => itemName;
        public ItemRarity Rarity => rarity;
        public ItemSortingOrder SortingOrder => sortingOrder;
        public GameObject Model => model;
        public int MaximumStack => maximumStack;
        public int CurrentStack => currentStack;

        public bool IsStackEmpty => currentStack == 0;
        public bool IsStackFull=> currentStack == maximumStack;
        public int RemainCapacity => maximumStack - currentStack;
        #endregion

        #region IComparable 인터페이스
        public int CompareTo(Item other)
        {
            if (sortingOrder != other.sortingOrder)
            {
                return sortingOrder - other.sortingOrder;
            }
            else if (rarity != other.rarity)
            {
                return other.rarity - rarity;
            }
            else if (itemName != other.itemName)
            {
                return itemName.CompareTo(other.itemName);
            }
            else
            {
                return other.currentStack - currentStack;
            }
        }
        #endregion

        /// <summary>
        /// 매개변수로 받은 아이템과 스택을 합치고 남은 아이템을 반환
        /// </summary>
        /// <param name="item">스택을 합칠 아이템</param>
        /// <returns>스택에 합치고 남은 아이템</returns>
        public Item MergeItemStack(Item item)
        {
            // TODO: Equals 검사
            if (itemName != item.itemName || item.Equals(this))
            {
                return item;
            }

            var remainCapacity = RemainCapacity;
            if (remainCapacity >= item.currentStack)
            {
                currentStack += item.currentStack;
                item.currentStack = 0;
            }
            else
            {
                currentStack += remainCapacity;
                item.currentStack -= remainCapacity;
            }

            return item;
        }

        public Item SplitItem(int stack)
        {
            var splitedItem = Instantiate(this);

            if (currentStack >= stack)
            {
                currentStack -= stack;
                splitedItem.currentStack = stack;
            }
            else
            {
                splitedItem.currentStack = currentStack;
                currentStack = 0;
            }

            return splitedItem;
        }
    }
}