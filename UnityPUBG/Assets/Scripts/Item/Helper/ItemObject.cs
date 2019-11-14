using UnityPUBG.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Utilities;
using UnityPUBG.Scripts.Logic;
using UnityEditor;

namespace UnityPUBG.Scripts.Items
{
    [RequireComponent(typeof(Rigidbody), typeof(PhotonView))]
    public class ItemObject : MonoBehaviour
    {
        [SerializeField, ReadOnly] private int id = -1;
        private Item item = null;

        private PhotonView photonView;

        public TextMesh textMesh;

        public Item Item
        {
            get { return item; }
            set
            {
                if (item == null || item.Data.ItemName.Equals(value.Data.ItemName) == false)
                {
                    DestroyAllChild();
                    SpawnItemModel(value);
                }

                item = value;
            }
        }
        public int PhotonViewId { get { return photonView.viewID; } }
        public GameObject ModelObject { get; private set; }
        public bool AllowAutoLoot { get; set; } = true;

        #region 유니티 메시지
        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }

        private void Update()
        {
            textMesh.text = Item.CurrentStack.ToString();
        }
        #endregion

        public void NotifyUpdateItem()
        {
            photonView.RPC(nameof(UpdateItem), PhotonTargets.Others, Item.Data.ItemName);
            photonView.RPC(nameof(UpdateCurrentStack), PhotonTargets.Others, Item.CurrentStack);
        }

        public void NotifyUpdateCurrentStack()
        {
            photonView.RPC(nameof(UpdateCurrentStack), PhotonTargets.Others, Item.CurrentStack);
        }

        public void RequestDestroy()
        {
            if (photonView.isMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                var ownerPlayer = PhotonPlayer.Find(photonView.ownerId);
                if (ownerPlayer == null)
                {
                    return;
                }

                photonView.RPC(nameof(Destroy), ownerPlayer);
                gameObject.SetActive(false);
            }
        }

        private void DestroyAllChild()
        {
            Destroy(ModelObject);
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void SpawnItemModel(Item item)
        {
            if (item.Data == null || item.Data.Model == null)
            {
                return;
            }

            ModelObject = Instantiate(item.Data.Model, transform);
        }

        [PunRPC]
        private void UpdateItem(string newItemDataName)
        {
            if (Item == null || Item.Data.ItemName != newItemDataName)
            {
                if (ItemDataCollection.Instance.ItemDataByName.TryGetValue(newItemDataName, out var newItemData))
                {
                    Item = newItemData.BuildItem();
                }
                else
                {
                    Debug.LogError($"{newItemData}와 일치하는 ItemName이 없습니다");
                }
            }
        }

        [PunRPC]
        private void UpdateCurrentStack(int newStack)
        {
            if (Item == null)
            {
                Debug.LogError($"업데이트 하려는 {nameof(Item)}이 null 입니다");
                return;
            }

            Item.SetStack(newStack);
        }

        [PunRPC]
        private void Destroy()
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
