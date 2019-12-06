using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.UI
{
    public class LootButton : PoolObject
    {
        public Image lootButtonImage;
        public Color defaultColor;
        public Color autoLootColor;

        private RectTransform rectTransform;
        private RectTransform holderRectTransform;
        private bool isAutoLootTarget = false;

        public ItemObject TargetItemObject { get; set; }
        public bool IsAutoLootTarget
        {
            get { return isAutoLootTarget; }
            set
            {
                isAutoLootTarget = value;
                lootButtonImage.color = value ? autoLootColor : defaultColor;
            }
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            holderRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (TargetItemObject == null)
            {
                SaveToPool();
                return;
            }

            var screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, TargetItemObject.transform.position);
            rectTransform.anchoredPosition = screenPoint - holderRectTransform.sizeDelta / 2f;
        }

        #region PoolObject
        public override void OnObjectReuse()
        {

        }

        public override void OnObjectSaveToPool()
        {
            if (TargetItemObject != null)
            {
                TargetItemObject.LootButton = null;
                TargetItemObject = null;
            }
        }
        #endregion

        public void LootItem()
        {
            if (EntityManager.Instance.MyPlayer == null || TargetItemObject == null)
            {
                return;
            }

            EntityManager.Instance.MyPlayer.LootItem(TargetItemObject);
            if (TargetItemObject == null || TargetItemObject.Item.IsStackEmpty)
            {
                SaveToPool();
            }
        }

        public void SaveToPool()
        {
            ObjectPoolManager.Instance.SaveUIObjectToPool(this);
        }
    }
}
