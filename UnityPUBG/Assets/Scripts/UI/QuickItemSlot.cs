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

        private void Start()
        {
            quickSlotIndex = transform.GetSiblingIndex();
        }
        #endregion

        /// <summary>
        /// 퀵슬롯 이미지, 인덱스 업데이트
        /// </summary>
        public void UpdateQuickItemSlot()
        {
            Item quickSlotItem = EntityManager.Instance.MyPlayer.ItemQuickBar[quickSlotIndex];
            slotImage.sprite = (quickSlotItem.IsStackEmpty == false) ? quickSlotItem.Data.Icon : defaultSlotSprite;
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