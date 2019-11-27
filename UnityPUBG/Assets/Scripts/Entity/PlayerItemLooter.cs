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
    public class PlayerItemLooter : MonoBehaviour
    {
        [Range(0.1f, 1f)]
        public float searchForLootPeriod = 0.2f;
        [Range(1f, 5f)]
        public float lootRadius = 2f;
        [Range(1f, 2f)]
        public float autoLootRadius = 1f;
        public LayerMask lootMask;

        [Header("Debug")]
        public bool showLootRadius = false;

        private Player player;
        public List<ItemObject> LootableItemObjects { get; private set; }

        #region 유니티 메시지
        private void Awake()
        {
            player = GetComponentInParent<Player>();
            if (player == null || player.IsMyPlayer == false)
            {
                Destroy(gameObject);
            }

            LootableItemObjects = new List<ItemObject>();
        }

        private void Start()
        {
            StartCoroutine(SearchLootableItem());
        }

        private void OnDrawGizmos()
        {
            if (showLootRadius)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, lootRadius);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, autoLootRadius);
            }
        }
        #endregion

        private bool IsAutoLootTarget(Item lootItem)
        {
            if (lootItem.IsStackEmpty)
            {
                return false;
            }

            // TODO: 실드 내구도가 더 많이 남아있는 경우도 true
            if (lootItem.Data is ArmorData)
            {
                if (player.EquipedArmor.IsStackEmpty || player.EquipedArmor.Data.Rarity < lootItem.Data.Rarity)
                {
                    return true;
                }
            }
            else if (lootItem.Data is BackpackData)
            {
                if (player.EquipedBackpack.IsStackEmpty || player.EquipedBackpack.Data.Rarity < lootItem.Data.Rarity)
                {
                    return true;
                }
            }
            else
            {
                var sameItemAtContainer = player.ItemContainer.TryGetItem(lootItem.Data.ItemName);
                if (sameItemAtContainer.IsStackEmpty == false && sameItemAtContainer.IsStackFull == false)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator SearchLootableItem()
        {
            float autoLootSquaredRadius = autoLootRadius * autoLootRadius;

            while (true)
            {
                LootableItemObjects.Clear();

                foreach (Collider collider in Physics.OverlapSphere(transform.position, lootRadius, lootMask))
                {
                    var collideItemObject = collider.gameObject.GetComponent<ItemObject>();
                    if (collideItemObject == null || collideItemObject.AllowLoot == false || collideItemObject.Item.IsStackEmpty)
                    {
                        continue;
                    }

                    if ((collideItemObject.transform.position - transform.position).sqrMagnitude <= autoLootSquaredRadius && IsAutoLootTarget(collideItemObject.Item))
                    {
                        player.LootItem(collideItemObject);
                    }
                    else
                    {
                        LootableItemObjects.Add(collideItemObject);
                    }
                }

                yield return new WaitForSeconds(searchForLootPeriod);
            }
        }
    }
}
