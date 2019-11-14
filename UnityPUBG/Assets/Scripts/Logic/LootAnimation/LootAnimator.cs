using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Items;

namespace UnityPUBG.Scripts.Logic
{
    [RequireComponent(typeof(PhotonView))]
    public class LootAnimator : Singleton<LootAnimator>
    {
        public LootAnimationSettings lootAnimationSettings;

        private PhotonView photonView;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }

        public void CreateNewLootAnimation(Player looter, ItemObject lootItemObject)
        {
            photonView.RPC(nameof(PlayLootAnimation), PhotonTargets.All, looter.PhotonViewId, lootItemObject.Item.Data.ItemName, lootItemObject.transform.position);
        }

        [PunRPC]
        private void PlayLootAnimation(int looterViewId, string lootItemName, Vector3 lootItemPosition)
        {
            var looterPhotonView = PhotonView.Find(looterViewId);
            if (looterPhotonView == null)
            {
                Debug.LogWarning($"해당 id와 일치하는 {nameof(photonView)}를 찾을 수 없습니다, {nameof(looterViewId)}: {looterViewId}");
                return;
            }

            if (ItemDataCollection.Instance.ItemDataByName.TryGetValue(lootItemName, out var lootItemData) == false)
            {
                Debug.LogWarning($"해당 이름의 {nameof(ItemData)}가 {nameof(ItemDataCollection)}에 없습니다, {nameof(lootItemName)}: {lootItemName}");
                return;
            }

            var lootAnimation = LootAnimation.InstantiateAnimation(looterPhotonView.transform, lootItemData.Model, lootItemPosition, lootAnimationSettings);
            if (lootAnimation != null)
            {
                lootAnimation.transform.parent = transform;
            }
        }
    }
}
