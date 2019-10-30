using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UnityPUBG.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class DamageText : MonoBehaviour
    {
        public Animator animator;
        public Text text;
        public Vector3 floatingOffset = new Vector3(0, 2.2f, 0);
        public float duration = 1f;

        [HideInInspector]
        public Transform damageReceiver;

        private Canvas canvas;
        private RectTransform rectTransform;
        private RectTransform canvasRectTransform;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (text == null)
            {
                text = GetComponentInChildren<Text>();
            }

            canvas = GetComponentInParent<Canvas>();
            rectTransform = GetComponent<RectTransform>();
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }

        private void Start()
        {
            Destroy(gameObject, duration);
        }

        private void LateUpdate()
        {
            var screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, damageReceiver.position + floatingOffset);
            rectTransform.anchoredPosition = screenPoint - canvasRectTransform.sizeDelta / 2f;
        }
    }
}