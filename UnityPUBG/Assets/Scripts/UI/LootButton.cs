using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts.UI
{
    public class LootButton : PoolObject
    {
        public ItemObject targetItemObject;

        private RectTransform rectTransform;
        private RectTransform holderRectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            holderRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (targetItemObject == null)
            {
                SaveToPool();
                return;
            }

            var screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, targetItemObject.transform.position);
            rectTransform.anchoredPosition = screenPoint - holderRectTransform.sizeDelta / 2f;
        }

        #region PoolObject
        public override void OnObjectReuse()
        {

        }

        public override void OnObjectSaveToPool()
        {
            if (targetItemObject != null)
            {
                targetItemObject.LootButton = null;
                targetItemObject = null;
            }
        }
        #endregion

        public void LootItem()
        {
            if (EntityManager.Instance.MyPlayer == null || targetItemObject == null)
            {
                return;
            }

            EntityManager.Instance.MyPlayer.LootItem(targetItemObject);
            if (targetItemObject == null || targetItemObject.Item.IsStackEmpty)
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
