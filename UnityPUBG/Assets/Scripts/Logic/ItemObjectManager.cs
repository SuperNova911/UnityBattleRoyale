using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;

namespace UnityPUBG.Scripts.Logic
{
    public class ItemObjectManager : Singleton<ItemObjectManager>
    {
        private Dictionary<int, ItemObject> itemObjects = new Dictionary<int, ItemObject>();
        private int nextId = 0; 

        public void AddToManageCollection(ItemObject itemObjectToAdd)
        {
            // 이미 등록된 ItemObject인지 검사
            if (itemObjectToAdd.Id >= 0)
            {
                if (itemObjects.ContainsKey(itemObjectToAdd.Id))
                {
                    Debug.LogWarning($"이미 Manager컬렉션에 등록된 {nameof(ItemObject)}입니다, Id: {itemObjectToAdd.Id}");
                    return;
                }
                else
                {
                    // 이 분기로 오는건 Id를 잘못 관리하고 있는 상황
                    throw new ArgumentException($"잘못된 Id: {itemObjectToAdd.Id}", nameof(itemObjectToAdd));
                }
            }

            itemObjectToAdd.Id = nextId++;
            itemObjects.Add(itemObjectToAdd.Id, itemObjectToAdd);

            NotifyAddToOtherClient(itemObjectToAdd.Id);
        }

        public void RemoveFromManageCollection(ItemObject itemObjectToRemove)
        {
            if (itemObjectToRemove.Id < 0)
            {
                Debug.LogWarning($"Manage 컬렉션에서 관리되고 있지 않은 {nameof(ItemObject)}를 Collection에서 지우려고 하고 있습니다");
                return;
            }

            bool removeResult = itemObjects.Remove(itemObjectToRemove.Id);
            if (removeResult)
            {
                NotifyRemoveToOtherClient(itemObjectToRemove.Id);
            }
            else
            {
                Debug.LogWarning($"지우려고 하는 {nameof(ItemObject)}가 Collection에서 이미 지워졌습니다");
            }

            itemObjectToRemove.Id = -1;
        }

        /// <summary>
        /// 매개변수로 받은 Id에 해당하는 ItemObject를 반환, 일치하는 ItemObject가 없으면 null 반환
        /// </summary>
        /// <param name="id">찾을 ItemObject의 Id</param>
        /// <returns>Id와 일치하는 ItemObject</returns>
        public ItemObject GetItemObjectById(int id)
        {
            if (id < 0)
            {
                Debug.LogError($"Id가 0보다 작을 수 없습니다, Id: {id}");
                return null;
            }

            return itemObjects[id];
        }

        public bool ContainsId(int id)
        {
            return itemObjects.ContainsKey(id);
        }

        private void NotifyAddToOtherClient(int id)
        {
            // TODO: 다른 클라이언트들에게 알리기
        }

        private void NotifyRemoveToOtherClient(int id)
        {
            // TODO: 다른 클라이언트들에게 알리기
        } 
    }
}
