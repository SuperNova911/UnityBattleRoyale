using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Logic
{
    public class ItemObjectManager : Singleton<ItemObjectManager>
    {
        private Dictionary<int, ItemObject> managedItemObjects = new Dictionary<int, ItemObject>();

        #region 유니티 메시지
        #endregion

        public void AddToManageCollection(ItemObject itemObjectToAdd)
        {
            int id = itemObjectToAdd.PhotonViewId;

            if (managedItemObjects.ContainsKey(id))
            {
                Debug.LogWarning($"이미 컬렉션에 등록된 {nameof(ItemObject)}입니다, {nameof(id)}: {id}");
                return;
            }

            managedItemObjects.Add(id, itemObjectToAdd);
        }

        public void RemoveFromManageCollection(int targetId)
        {
            if (managedItemObjects.ContainsKey(targetId) == false)
            {
                Debug.LogWarning($"관리되고 있지 않은 {nameof(ItemObject)}를 컬렉션에서 지우려고 하고 있습니다, {nameof(targetId)}: {targetId}");
                return;
            }

            managedItemObjects.Remove(targetId);
        }

        /// <summary>
        /// 매개변수로 받은 Id에 해당하는 ItemObject를 반환, 일치하는 ItemObject가 없으면 null 반환
        /// </summary>
        /// <param name="id">찾을 ItemObject의 Id</param>
        /// <returns>Id와 일치하는 ItemObject</returns>
        public ItemObject FindItemObjectById(int id)
        {
            if (managedItemObjects.TryGetValue(id, out var itemObject))
            {
                return itemObject;
            }
            else
            {
                return null;
            }
        }

        public bool ContainsId(int id)
        {
            return managedItemObjects.ContainsKey(id);
        }
        
        #region RPC 함수
        #endregion
    }
}
