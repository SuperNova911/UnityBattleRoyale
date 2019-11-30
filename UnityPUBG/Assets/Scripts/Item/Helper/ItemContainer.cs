using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Logic;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Items
{
    public class ItemContainer
    {
        private List<Item> container;

        /// <summary>
        /// 매개변수로 받은 값의 크기를 용량으로 가진 아이템 컨테이너를 생성
        /// </summary>
        /// <param name="initialCapacity">컨테이너의 크기</param>
        public ItemContainer(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                Debug.LogError($"{nameof(ItemContainer)}의 크기가 0보다 작을 수 없습니다, {nameof(initialCapacity)}: {initialCapacity}");
                initialCapacity = 0;
            }
            container = new List<Item>(initialCapacity);
        }

        public event EventHandler OnContainerUpdate;
        public event EventHandler OnContainerResize;

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
                Debug.LogWarning($"비어있는 아이템은 컨테이너에 넣을 수 없습니다, {nameof(itemToAdd.Data.ItemName)}: {itemToAdd.Data.ItemName}");
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

            OnContainerUpdate?.Invoke(this, EventArgs.Empty);
            return itemToAdd;
        }

        public Item FillItem(Item itemToFill)
        {
            if (itemToFill == null)
            {
                Debug.LogError("null인 아이템은 컨테이너에 넣을 수 없습니다");
                return itemToFill;
            }

            if (itemToFill.IsStackEmpty)
            {
                Debug.LogWarning($"비어있는 아이템은 컨테이너에 넣을 수 없습니다, {nameof(itemToFill.Data.ItemName)}: {itemToFill.Data.ItemName}");
                return itemToFill;
            }

            foreach (var targetItem in container.Where(e => e.Data.ItemName == itemToFill.Data.ItemName && e.IsStackFull == false))
            {
                itemToFill = targetItem.MergeStack(itemToFill);
                if (itemToFill.IsStackEmpty)
                {
                    break;
                }
            }

            OnContainerUpdate?.Invoke(this, EventArgs.Empty);
            return itemToFill;
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

            OnContainerUpdate?.Invoke(this, EventArgs.Empty);

            return splitedItem;
        }

        /// <summary>
        /// ItemData의 ItemName으로 해당 아이템 존재 유무 검사
        /// </summary>
        /// <param name="itemName">찾아 볼 ItemName</param>
        /// <returns>해당 아이템이 있으면 true</returns>
        public bool HasItem(string itemName)
        {
            return container.Any(e => e.Data.ItemName == itemName);
        }

        /// <summary>
        /// 매개변수로 받은 슬롯의 아이템을 반환, 슬롯이 비어있으면 EmptyItem 반환
        /// </summary>
        /// <param name="slot">아이템의 슬롯 번호</param>
        /// <returns>입력으로 받은 슬롯의 아이템</returns>
        public Item GetItemAt(int slot)
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

        /// <summary>
        /// 입력으로 받은 ItemName과 일치하는 컨테이너 아이템의 슬롯 번호를 반환, 뒤에서부터 검색
        /// </summary>
        /// <param name="itemName">검색할 ItemName</param>
        /// <returns>일치하는 아이템의 슬롯 번호</returns>
        public int FindMatchItemSlotFromLast(string itemName)
        {
            for (int slot = Count - 1; slot >= 0; slot--)
            {
                if (container[slot].Data.ItemName.Equals(itemName))
                {
                    return slot;
                }
            }

            return -1;
        }

        /// <summary>
        /// 입력으로 받은 ItemName과 일치하는 컨테이너의 모든 아이템을 반환
        /// </summary>
        /// <param name="itemName">검색할 ItemName</param>
        /// <returns>일치하는 아이템 리스트</returns>
        public List<Item> FindAllMatchItem(string itemName)
        {
            var matchItems = new List<Item>();
            foreach (var item in container)
            {
                if (item.IsStackEmpty == false && item.Data.ItemName.Equals(itemName))
                {
                    matchItems.Add(item);
                }
            }

            return matchItems;
        }

        /// <summary>
        /// 아이템 데이터 이름과 일치하는 아이템을 뒤에서부터 찾아서 반환, 일치하는 아이템이 없으면 EmptyItem 반환 
        /// </summary>
        /// <param name="itemName">검색 할 아이템 데이터의 아이템 이름</param>
        /// <returns>일치하는 아이템 또는 EmptyItem</returns>
        public Item TryGetItemFromLast(string itemName)
        {
            var matchItem = container.LastOrDefault(e => e.Data.ItemName.Equals(itemName));
            return matchItem ?? Item.EmptyItem;
        }

        /// <summary>
        /// 컨테이너의 크기를 늘이거나 줄이고 넘친 아이템들은 반환
        /// </summary>
        /// <param name="newCapacity">새로운 컨테이너의 크기</param>
        /// <returns>컨테이너 크기를 변경한 후 넘친 아이템</returns>
        public List<Item> ResizeCapacity(int newCapacity)
        {
            var overflowItems = new List<Item>();

            if (newCapacity < 0)
            {
                Debug.LogError($"{nameof(ItemContainer)}의 크기가 0보다 작을 수 없습니다, {nameof(newCapacity)}: {newCapacity}");
                newCapacity = Mathf.Clamp(newCapacity, 0, int.MaxValue);
            }

            var newContainer = new List<Item>(newCapacity);
            for (int index = 0; index < Count && index < Capacity; index++)
            {
                if (index < newCapacity)
                {
                    newContainer.Add(container[index]);
                }
                else
                {
                    overflowItems.Add(container[index]);
                }
            }

            container = newContainer;
            OnContainerResize?.Invoke(this, EventArgs.Empty);
            OnContainerUpdate?.Invoke(this, EventArgs.Empty);
            return overflowItems;
        }
    }
}
