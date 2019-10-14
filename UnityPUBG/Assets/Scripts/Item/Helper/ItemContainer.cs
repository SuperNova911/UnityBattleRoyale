using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/Inventory")]
    public class ItemContainer : ScriptableObject
    {
        #region 필드
        [SerializeField] private int capacity = 6;
        public List<Item> container;
        #endregion

        #region 속성
        public bool IsEmpty => container.Count == 0;
        public bool IsFull => container.Count == capacity;
        public int RemainCapacity => capacity - container.Count;
        #endregion

        #region 유니티 메시지
        private void OnEnable()
        {
            container = new List<Item>(capacity);
        }
        #endregion

        #region 메서드
        /// <summary>
        /// 매개변수로 받은 아이템을 컨테이너에 넣을 수 있는 만큼 넣고 남은 아이템을 반환
        /// </summary>
        /// <param name="itemToAdd">컨테이너에 넣을 아이템</param>
        /// <returns>컨테이너에 넣고 남은 아이템</returns>
        public Item AddItem(Item itemToAdd)
        {
            if (itemToAdd == null)
            {
                Debug.LogError("null인 아이템은 컨테이너에 넣을 수 없습니다");
                return itemToAdd;
            }

            if (itemToAdd.IsStackEmpty)
            {
                Debug.LogWarning("비어있는 아이템은 컨테이너에 넣을 수 없습니다");
                return itemToAdd;
            }

            foreach (var targetItem in container.Where(e => e.Data.ItemName == itemToAdd.Data.ItemName && e.IsStackFull == false))
            {
                itemToAdd = targetItem.MergeStack(itemToAdd);
                if (itemToAdd.IsStackEmpty)
                {
                    return itemToAdd;
                }
            }

            if (itemToAdd.IsStackEmpty == false && IsFull == false)
            {
                container.Add(itemToAdd);
                container.Sort();

                return Item.EmptyItem;
            }

            return itemToAdd;
        }

        public Item SubtrackItemAtSlot(int slot)
        {
            return SubtrackItemsAtSlot(slot, 1);
        }

        public Item SubtrackItemsAtSlot(int slot, int stack)
        {
            if (slot >= container.Count)
            {
                return Item.EmptyItem;
            }

            Item slotItem = container[slot];
            stack = Mathf.Clamp(stack, 0, slotItem.CurrentStack);

            Item splitedItem = slotItem.SplitStack(stack);

            if (slotItem.IsStackEmpty)
            {
                container.RemoveAt(slot);
            }

            return splitedItem;
        }

        public bool HasItem(string itemName)
        {
            return container.Any(e => e.Data.ItemName == itemName);
        }

        public Item FindItem(string itemName)
        {
            return container.FirstOrDefault(e => e.Data.ItemName == itemName);
        }
        #endregion
    }
}
