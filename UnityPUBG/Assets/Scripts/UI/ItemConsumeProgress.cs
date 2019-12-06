using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.UI
{
    public class ItemConsumeProgress : MonoBehaviour
    {
        public Button interruptButton;
        public Image itemIcon;
        public TMP_Text itemName;
        public Slider progressSlider;
        public Image progressSliderFill;
        public TMP_Text remainTimeText;

        private void Awake()
        {
            interruptButton.onClick.AddListener(() => InterruptProgress());
        }

        public void InitializeProgress(ConsumableData consumableData)
        {
            itemIcon.sprite = consumableData.Icon;
            itemName.text = consumableData.ItemName;
            progressSlider.value = 0f;
            progressSliderFill.color = ItemDataCollection.Instance.ItemColorsByRarity[consumableData.Rarity].Color;
            remainTimeText.text = consumableData.TimeToUse.ToString("0.0");

            gameObject.SetActive(true);
        }

        public void UpdateProgress(float progress, float remainTime)
        {
            progressSlider.value = progress;
            remainTimeText.text = remainTime.ToString("0.0");
        }

        public void Clear()
        {
            gameObject.SetActive(false);
        }

        private void InterruptProgress()
        {
            var targetPlayer = EntityManager.Instance.MyPlayer;
            if (targetPlayer != null)
            {
                targetPlayer.InterruptItemUse();
            }
        }
    }
}
