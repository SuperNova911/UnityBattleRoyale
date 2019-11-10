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

            //if (PhotonNetwork.isMasterClient)
            //    NotifyAddToOtherClient(itemObjectToAdd.Id);
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

        /// <summary>
        /// 마스터 클라이언트가
        /// 스폰 포인트에서
        /// 생성한 게임 오브젝트를
        /// 다른 클라이언트들에게 알려줌
        /// </summary>
        /// <param name="spawnPoints">아이템 오브젝트를 생성한 스폰 포인트</param>
        public void SendMasterNotifyAddToOtherClient(List<ItemSpawnPoint> spawnPoints)
        {
            //마스터 클라이언트가 아닐 시 종료
            if (!PhotonNetwork.isMasterClient)
                return;

            //스폰 포인트 이름들
            List<string> spawnPointNames = new List<string>();

            //아이템 오브젝트 이름들
            List<string> itemObjectNames = new List<string>();

            //아이템 오브젝트 ID들
            List<int> itemObjectIDs = new List<int>();

            int count = spawnPoints.Count;
            ItemSpawnPoint spawnPoint = null;

            for(int i = 0; i<count; i++)
            {
                spawnPoint = spawnPoints[i];

                //스폰 포인트 이름 저장
                spawnPointNames.Add(spawnPoint.name);
                //아이템 오브젝트 이름 저장
                itemObjectNames.Add(spawnPoint.SpawnedItem.Item.Data.ItemName);
                //아이템 오브젝트 ID 저장
                itemObjectIDs.Add
                    (itemObjects.FirstOrDefault
                    (x => x.Value == spawnPoint.SpawnedItem).Key);
            }

            //RPC 함수로 다른 클라이언트에게 생성한 아이템을 알려줌
            GetComponent<PhotonView>().RPC
                ("MasterNotifyAddToOtherClient", PhotonTargets.Others, 
                spawnPointNames.ToArray(), itemObjectNames.ToArray(), itemObjectIDs.ToArray());
        }

        private void NotifyAddToOtherClient(int id)
        {
            // TODO: 다른 클라이언트들에게 알리기
        }

        private void NotifyRemoveToOtherClient(int id)
        {
            // TODO: 다른 클라이언트들에게 알리기
        }

        #region RPC 함수
        /// <summary>
        /// 마스터 클라이언트가 알려준
        /// <para>스폰 포인트 이름,</para>
        /// <para>아이템 오브젝트 이름,</para>
        /// <para>ID를 통해서</para>
        /// <para>아이템 오브젝트를 스폰하고, ID를 부여</para>
        /// </summary>
        /// <param name="spawnPointNames">스폰 포인트 이름</param>
        /// <param name="itemObjectNames">아이템 오브젝트 이름</param>
        /// <param name="ItemObjectIDs">아이템 오브젝트 ID</param>
        [PunRPC]
        private void MasterNotifyAddToOtherClient
            (string[] spawnPointNames, string[] itemObjectNames, int[] ItemObjectIDs)
        {
            int length = spawnPointNames.Length;
            ItemObject itemObject = null;
            for(int i = 0; i<length; i++)
            {
                //아이템 오브젝트 생성
                itemObject = ItemSpawnManager.Instance.SpawnItem
                    (spawnPointNames[i], itemObjectNames[i]);

                //ID 부여
                itemObjects.Add(ItemObjectIDs[i], itemObject);
            }
        }
        #endregion
    }
}
