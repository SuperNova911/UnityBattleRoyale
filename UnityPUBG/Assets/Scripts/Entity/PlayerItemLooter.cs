using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.Entities
{
    [RequireComponent(typeof(SphereCollider))]
    public class PlayerItemLooter : MonoBehaviour
    {
        [Range(0.5f, 5f)]
        public SphereCollider lootCollider;
        public float lootRadius = 2f;

        private Player player;

        #region 유니티 메시지
        private void Awake()
        {
            player = GetComponentInParent<Player>();
            if (player == null)
            {
                Debug.LogError($"{nameof(PlayerItemLooter)}는 부모 오브젝트의 {nameof(Player)} 컴포넌트가 필요합니다");
            }
        }

        private void Start()
        {
            if (player == null || player.IsMyPlayer == false)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("ItemObject"))
            {
                var targetItemObject = other.GetComponent<ItemObject>();
                if (targetItemObject != null)
                {
                    AutoLootItem(targetItemObject);
                }
            }
        }

        private void OnValidate()
        {
            if (lootCollider == null)
            {
                lootCollider = GetComponent<SphereCollider>();
            }
            lootCollider.isTrigger = true;
            lootCollider.radius = lootRadius;
        }
        #endregion

        private void AutoLootItem(ItemObject lootItemObject)
        {
            if (lootItemObject.AllowAutoLoot == false)
            {
                return;
            }

            if (lootItemObject.Item == null)
            {
                return;
            }

            int previousStack = lootItemObject.Item.CurrentStack;
            var remainItem = player.ItemContainer.AddItem(lootItemObject.Item);

            if (previousStack != remainItem.CurrentStack)
            {
                LootAnimator.Instance.CreateNewLootAnimation(player, lootItemObject);
                lootItemObject.Item = remainItem;

                if (lootItemObject.Item.IsStackEmpty)
                {
                    lootItemObject.RequestDestroy();
                }
                else
                {
                    lootItemObject.NotifyUpdateCurrentStack();
                }
            }
        }
    }
}
