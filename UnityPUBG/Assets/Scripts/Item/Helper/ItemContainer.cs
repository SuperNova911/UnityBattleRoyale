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
    public class ItemContainer
    {
        private readonly List<Item> container;

        /// <summary>
        /// 매개변수로 받은 값의 크기를 용량으로 가진 아이템 컨테이너를 생성
        /// </summary>
        /// <param name="capacity">컨테이너의 크기</param>
        public ItemContainer(int capacity)
        {
            if (capacity < 0)
            {
                Debug.LogError($"컨테이너의 크기가 0보다 작을 수 없습니다, capacity: {capacity}");
                capacity = 0;
            }
            container = new List<Item>(capacity);
        }

        public event EventHandler OnUpdateContainer;

        public int Count => container.Count;
        public int Capacity => container.Capacity;
        public int RemainCapacity => container.Capacity - container.Count;
        public bool IsEmpty => container.Count == 0;
        public bool IsFull => container.Count == container.Capacity;

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
                    break;
                }
            }

            if (itemToAdd.IsStackEmpty == false && IsFull == false)
            {
                container.Add(itemToAdd);
                container.Sort();

                itemToAdd = Item.EmptyItem;
            }

            if (OnUpdateContainer != null)
            {
                OnUpdateContainer(this, EventArgs.Empty);
            }
            return itemToAdd;
        }

        /// <summary>
        /// 매개변수로 받은 슬롯에 해당하는 아이템 하나를 컨테이너에서 빼고 반환
        /// </summary>
        /// <param name="slot">컨테이너의 슬롯 인덱스</param>
        /// <returns>컨테이너에서 뺀 아이템</returns>
        public Item SubtrackItemAtSlot(int slot)
        {
            return SubtrackItemsAtSlot(slot, 1);
        }

        /// <summary>
        /// 매개변수로 받은 슬롯과 스택 크기만큼 아이템을 컨테이너에서 빼고 반환
        /// </summary>
        /// <param name="slot">컨테이너의 슬롯 인덱스</param>
        /// <param name="stack">아이템을 뺄 스택 크기</param>
        /// <returns>컨테이너에서 뺀 아이템</returns>
        public Item SubtrackItemsAtSlot(int slot, int stack)
        {
            if (slot < 0 || container.Capacity <= slot)
            {
                Debug.LogError($"컨테이너의 범위를 초과한 슬롯 인덱스 입니다, slot: {slot}");
                return Item.EmptyItem;
            }

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

            if (OnUpdateContainer != null)
            {
                OnUpdateContainer(this, EventArgs.Empty);
            }
            return splitedItem;
        }

        public bool HasItem(string itemName)
        {
            return container.Any(e => e.Data.ItemName == itemName);
        }

        public Item FindItem(int slot)
        {
            if (slot < 0 || container.Capacity <= slot)
            {
                Debug.LogError($"컨테이너의 범위를 초과한 슬롯 인덱스 입니다, slot: {slot}");
                return Item.EmptyItem;
            }

            if (slot >= container.Count)
            {
                return Item.EmptyItem;
            }

            return container[slot];
        }
    }
}
