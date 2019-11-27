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
    [RequireComponent(typeof(Animator))]
    public class DamageText : PoolObject
    {
        [Header("UI")]
        public TMP_Text text;

        [Header("Animation")]
        [Range(1f, 2f)] public float animationDuration = 1f;
        public Vector3 floatingOffset = new Vector3(0, 2.2f, 0);

        private Canvas canvas;
        private RectTransform rectTransform;
        private RectTransform canvasRectTransform;

        public Transform DamageReceiver { private get; set; }

        #region 유니티 메시지
        private void Awake()
        {
            if (text == null)
            {
                text = GetComponentInChildren<TMP_Text>();
            }

            canvas = GetComponentInParent<Canvas>();
            rectTransform = GetComponent<RectTransform>();
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, DamageReceiver.position + floatingOffset);
            rectTransform.anchoredPosition = screenPoint - canvasRectTransform.sizeDelta / 2f;
        } 
        #endregion

        #region PoolObject
        public override void OnObjectReuse()
        {
            Invoke(nameof(SaveToPool), animationDuration);
        }

        public override void OnObjectSaveToPool()
        {

        }
        #endregion

        private void SaveToPool()
        {
            ObjectPoolManager.Instance.SaveUIObjectToPool(this);
        }
    }
}