using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts
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
        /// <para>모든 아이템을 컨테이너에 넣었으면 null을 반환</para>
        /// </summary>
        /// <param name="item">컨테이너에 넣을 아이템</param>
        /// <returns>컨테이너에 넣고 남은 아이템</returns>
        public Item AddItem(Item item)
        {
            if (item == null)
            {
                Debug.LogWarning("유효하지 않은 아이템 입력입니다");
                return null;
            }

            foreach (var targetItem in container.Where(e => e.ItemName == item.ItemName && e.IsStackFull == false))
            {
                item = targetItem.MergeItemStack(item);
                if (item.CurrentStack <= 0)
                {
                    return null;
                }
            }

            if (IsFull == false)
            {
                container.Add(item);
                container.Sort();
                return null;
            }

            return item;
        }

        public Item SubtrackItemAtSlot(int slot)
        {
            return SubtrackItemsAtSlot(slot, 1);
        }

        public Item SubtrackItemsAtSlot(int slot, int stack)
        {
            if (slot >= container.Count)
            {
                return null;
            }

            var slotItem = container[slot];

            if (slotItem.CurrentStack > stack)
            {
                return slotItem.SplitItem(stack);
            }
            else
            {
                container.RemoveAt(slot);
                return slotItem;
            }
        }

        public bool HasItem(string itemName)
        {
            return container.Any(e => e.ItemName == itemName);
        }

        public Item FindItem(string itemName)
        {
            return container.FirstOrDefault(e => e.ItemName == itemName);
        }
        #endregion
    }
}
