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
using Knife.PostProcessing;
using UnityPUBG.Scripts.UI;

namespace UnityPUBG.Scripts.Items
{
    [RequireComponent(typeof(Rigidbody), typeof(PhotonView))]
    public class ItemObject : MonoBehaviour
    {
        [Range(1f, 3f)]
        public float readyToLootDelay = 1f;

        private Item item = null;
        private PhotonView photonView;
        private OutlineRegister outlineRegister;

        public Item Item
        {
            get { return item; }
            set
            {
                if (value == null)
                {
                    Debug.LogError($"{nameof(Item)}에는 null값을 할당할 수 없습니다, {nameof(PhotonViewId)}: {PhotonViewId}");
                    return;
                }

                var previoudItem = item;
                item = value;
                if (previoudItem == null || item.Data.ItemName.Equals(previoudItem.Data.ItemName) == false)
                {
                    DestroyAllChild();
                    SpawnItemModel(item);
                }
            }
        }
        public GameObject ModelPrefab { get; private set; }
        public bool AllowLoot { get; private set; }
        public LootButton LootButton { get; set; }
        public int PhotonViewId { get { return photonView.viewID; } }

        #region 유니티 메시지
        private void Awake()
        {
            photonView = GetComponent<PhotonView>();

            AllowLoot = false;
            LootButton = null;
        }

        private void Start()
        {
            Invoke(nameof(ReadyToLoot), readyToLootDelay);
        }

        private void OnDestroy()
        {
            if (LootButton != null)
            {
                LootButton.SaveToPool();
            }
        }
        #endregion

        public void NotifyUpdateItem()
        {
            switch (Item)
            {
                case ShieldItem shieldItem:
                    photonView.RPC(nameof(UpdateShieldItem), PhotonTargets.Others, Item.Data.ItemName, shieldItem.CurrentShield);
                    break;
                default:
                    photonView.RPC(nameof(UpdateItem), PhotonTargets.Others, Item.Data.ItemName);
                    break;
            }

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
            Destroy(ModelPrefab);
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

            ModelPrefab = Instantiate(item.Data.Model, transform);

            outlineRegister = ModelPrefab.AddComponent<OutlineRegister>();
            var rarityColor = ItemDataCollection.Instance.ItemColorsByRarity[item.Data.Rarity].Color;
            outlineRegister.SetTintColor(rarityColor);
        }

        private void ReadyToLoot()
        {
            AllowLoot = true;
        }

        [PunRPC]
        private void UpdateItem(string newItemDataName)
        {
            if (Item == null || Item.Data.ItemName != newItemDataName)
            {
                if (ItemDataCollection.Instance.ItemDataByName.TryGetValue(newItemDataName, out var newItemData))
                {
                    Item = new Item(newItemData);
                }
                else
                {
                    Debug.LogError($"{newItemData}와 일치하는 ItemName이 없습니다");
                }
            }
        }

        [PunRPC]
        private void UpdateShieldItem(string newItemDataName, float currentShield)
        {
            if (Item == null || Item.Data.ItemName != newItemDataName)
            {
                if (ItemDataCollection.Instance.ItemDataByName.TryGetValue(newItemDataName, out var newItemData))
                {
                    if (newItemData is ArmorData)
                    {
                        Item = new ShieldItem(newItemData as ArmorData, currentShield);
                    }
                    else
                    {
                        Debug.LogError($"{newItemData.ItemName}은 {nameof(ArmorData)}가 아닙니다");
                    }
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
