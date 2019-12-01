using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.UI
{
    public class QuickItemSlot : MonoBehaviour
    {
        /// <summary>
        /// 퀵슬롯 인덱스
        /// </summary>
        public int quickSlotIndex;
        public Button actionButton;
        /// <summary>
        /// 빈 슬롯 이미지
        /// </summary>
        public Image slotImage;
        public Sprite defaultSlotSprite;

        #region Unity 콜백
        private void Awake()
        {
            actionButton.onClick.AddListener(() => Button_OnClick());
        }
        #endregion

        /// <summary>
        /// 퀵슬롯 이미지, 인덱스 업데이트
        /// </summary>
        public void UpdateQuickItemSlot()
        {
            ItemData quickSlotItemData = EntityManager.Instance.MyPlayer.ItemQuickBar[quickSlotIndex];
            if (quickSlotItemData != null && EntityManager.Instance.MyPlayer.ItemContainer.HasItem(quickSlotItemData.ItemName))
            {
                slotImage.sprite = quickSlotItemData.Icon;
            }
            if (EntityManager.Instance.MyPlayer.ItemContainer.HasItem(quickSlotItemData.ItemName))
            {
                slotImage.sprite = defaultSlotSprite;
            }
        }

        public void Button_OnClick()
        {
            var targetPlayer = EntityManager.Instance.MyPlayer;
            if (targetPlayer != null)
            {
                targetPlayer.UseItemAtQuickBar(quickSlotIndex);
            }
        }
    }
}