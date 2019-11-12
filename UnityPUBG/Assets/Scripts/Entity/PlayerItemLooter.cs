using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Entities
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerItemLooter : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Player player;
        [SerializeField, ReadOnly] private SphereCollider lootCollider;
        [SerializeField, Range(0.5f, 5f)] private float lootRadius = 2f;

        [Header("Loot Animation")]
        [SerializeField] private LootAnimationSettings lootAnimationSettings;

        #region 유니티 메시지
        private void Awake()
        {
            if (player == null)
            {
                player = GetComponentInParent<Player>();
                if (player == null)
                {
                    Debug.LogError($"{nameof(PlayerItemLooter)}는 {nameof(Player)}가 포함된 부모 게임오브젝트가 필요합니다");
                }
            }

            if (lootAnimationSettings == null)
            {
                Debug.LogWarning($"{nameof(lootAnimationSettings)}이 자동으로 기본값으로 설정되었습니다");
                lootAnimationSettings = ScriptableObject.CreateInstance<LootAnimationSettings>();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (player.photonView.isMine)
            {
                if (other.tag == "ItemObject")
                {
                    var targetItemObject = other.GetComponent<ItemObject>();
                    if (targetItemObject != null)
                    {
                        AutoLootItem(targetItemObject);
                    }
                }
            }
        }

        private void OnValidate()
        {
            player = GetComponentInParent<Player>();

            if (lootCollider == null)
            {
                lootCollider = GetComponent<SphereCollider>();
            }
            lootCollider.isTrigger = true;
            lootCollider.radius = lootRadius;

            if (lootAnimationSettings == null)
            {
                Debug.LogWarning($"{nameof(lootAnimationSettings)}의 값이 null 입니다");
            }
        }
        #endregion

        private void AutoLootItem(ItemObject targetItemObject)
        {
            if (targetItemObject.AllowAutoLoot == false)
            {
                return;
            }

            var targetItem = targetItemObject.Item;
            if (targetItem == null)
            {
                return;
            }

            int previousStack = targetItem.CurrentStack;
            player.ItemContainer.AddItem(targetItem);

            if (previousStack != targetItem.CurrentStack)
            {
                player.photonView.RPC(nameof(PlayLootAnimation), PhotonTargets.Others, player.photonView.viewID, targetItemObject.PhotonViewId);
                LootAnimator.InstantiateAnimation(player.transform, targetItemObject.ModelObject, lootAnimationSettings);

                if (targetItem.IsStackEmpty)
                {
                    Destroy(targetItemObject.gameObject);
                }
                else
                {
                    targetItemObject.NotifyUpdateCurrentStack();
                }
            }
        }

        [PunRPC]
        private void PlayLootAnimation(int playerViewId, int itemObjectViewId)
        {
            var targetPlayerTransform = PhotonView.Find(playerViewId).transform;
            var targetModelObject = PhotonView.Find(itemObjectViewId).transform.GetComponent<ItemObject>().ModelObject;

            LootAnimator.InstantiateAnimation(targetPlayerTransform, targetModelObject, lootAnimationSettings);
        }
    }
}
