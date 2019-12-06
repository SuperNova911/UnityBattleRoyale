using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.Items
{
    public class Item : IComparable<Item>, ICloneable
    {
        private static EmptyItem emptyItem = null;
        public static EmptyItem EmptyItem
        {
            get
            {
                if (emptyItem == null)
                {
                    emptyItem = new EmptyItem(ItemDataCollection.Instance.EmptyItemData);
                }

                return emptyItem;
            }
        }

        private ItemData data;
        private int currentStack;

        public Item(ItemData data)
        {
            if (data != null)
            {
                this.data = data;
                CurrentStack = data.DefaultStack;
            }
        }

        public ItemData Data => data;
        public int CurrentStack
        {
            get { return currentStack; }
            protected set
            {
                currentStack = value;
                if (currentStack < 0 || data.MaximumStack < currentStack)
                {
                    Debug.LogError($"허용된 스택의 범위를 벗어난 값입니다, value: {value}");
                    currentStack = Mathf.Clamp(currentStack, 0, data.MaximumStack);
                }
            }
        }

        public bool IsStackEmpty => CurrentStack == 0;
        public bool IsStackFull => CurrentStack == data.MaximumStack;
        public int RemainCapacity => data.MaximumStack - CurrentStack;

        #region IComparable 인터페이스
        public int CompareTo(Item other)
        {
            if (data.SortingOrder != other.data.SortingOrder)
            {
                return data.SortingOrder.CompareTo(other.data.SortingOrder);
            }
            else if (data.Rarity != other.data.Rarity)
            {
                return other.data.Rarity.CompareTo(data.Rarity);
            }
            else if (data.ItemName != other.data.ItemName)
            {
                return data.ItemName.CompareTo(other.data.ItemName);
            }
            else
            {
                return other.currentStack.CompareTo(currentStack);
            }
        }
        #endregion

        #region ICloneable 인터페이스
        public object Clone()
        {
            var cloneItem = new Item(Data) { CurrentStack = CurrentStack };
            return cloneItem;
        }
        #endregion

        /// <summary>
        /// 매개변수로 받은 아이템과 스택을 병합하고 남은 아이템을 반환
        /// </summary>
        /// <param name="itemToMerge">스택을 병합 할 아이템</param>
        /// <returns>스택을 병합하고 남은 아이템</returns>
        public Item MergeStack(Item itemToMerge)
        {
            if (itemToMerge == null || itemToMerge is EmptyItem)
            {
                Debug.LogWarning("유효하지 않은 아이템은 Merge 할 수 없습니다");
                return itemToMerge;
            }

            if (itemToMerge.Equals(this))
            {
                Debug.LogWarning("자기 자신을 Merge 할 수는 없습니다");
                return itemToMerge;
            }

            int mergeSize = Mathf.Clamp(itemToMerge.CurrentStack, 0, RemainCapacity);
            itemToMerge.CurrentStack -= mergeSize;
            CurrentStack += mergeSize;

            return itemToMerge;
        }

        /// <summary>
        /// 매개변수로 입력 받은 크기만큼 아이템의 스택을 분할하여 반환
        /// </summary>
        /// <param name="splitSize">분할할 스택의 크기</param>
        /// <returns>스택이 분할된 아이템</returns>
        public Item SplitStack(int splitSize)
        {
            if (splitSize < 0)
            {
                Debug.LogWarning($"분할 하려는 스택의 크기가 너무 작습니다, splitSize: {splitSize}");
            }

            splitSize = Mathf.Clamp(splitSize, 0, CurrentStack);
            Item splitedItem = (Item)Clone();
            splitedItem.CurrentStack = splitSize;
            CurrentStack -= splitSize;

            return splitedItem;
        }

        // 직접 호출하면 안됨
        public void SetStack(int newStack)
        {
            CurrentStack = newStack;
        }

        public void ClearStack()
        {
            CurrentStack = 0;
        }
    }
}
