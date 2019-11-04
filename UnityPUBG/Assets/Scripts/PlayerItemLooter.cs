using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Utilities;

namespace unitu.Scripts
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerItemLooter : MonoBehaviour
    {
        #region 필드
        [SerializeField, ReadOnly] private Player player;
        [SerializeField, ReadOnly] private SphereCollider lootCollider;
        [SerializeField, Range(0.5f, 5f)] private float lootRadius = 2f;

        [Header("Loot Animation")]
        [SerializeField] private LootAnimationSettings lootAnimationSettings;
        #endregion

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
            if (other.tag == "ItemObject")
            {
                var itemObject = other.GetComponent<ItemObject>();
                if (itemObject != null && itemObject.Item != null)
                {
                    int previousStack = itemObject.Item.CurrentStack;
                    var remainItem = player.ItemContainer.AddItem(itemObject.Item);

                    if (previousStack > remainItem.CurrentStack)
                    {
                        LootAnimator.InstantiateAnimation(player.transform, itemObject.ModelObject, lootAnimationSettings);
                    }

                    if (remainItem.IsStackEmpty)
                    {
                        Destroy(itemObject.gameObject);
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
    }
}
