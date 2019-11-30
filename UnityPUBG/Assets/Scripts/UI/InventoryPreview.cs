using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.Items;

namespace UnityPUBG.Scripts.UI
{
    public class InventoryPreview : MonoBehaviour
    {
        public List<Image> previewImages = new List<Image>();
        public Color freeSlotColor;
        public Color hasItemColor;
        public Color itemAlmostFullColor;

        private int currentCount = 0;
        private int currentCapacity = 0;
        private float hideTime = 0;

        private void Awake()
        {
            foreach (var image in previewImages)
            {
                image.enabled = false;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(HidePreview(0.2f));
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void UpdatePreview(int newCount, int newCapacity)
        {
            if (currentCapacity != newCapacity || currentCount <= newCount)
            {
                for (int i = 0; i < previewImages.Count; i++)
                {
                    if (i < newCount)
                    {
                        previewImages[i].enabled = true;
                        previewImages[i].color = newCapacity - newCount <= 1 ? itemAlmostFullColor : hasItemColor;
                    }
                    else if (i < newCapacity)
                    {
                        previewImages[i].enabled = true;
                        previewImages[i].color = freeSlotColor;
                    }
                    else
                    {
                        previewImages[i].enabled = false;
                    }
                }
            }
            currentCount = newCount;
            currentCapacity = newCapacity;

            hideTime = Time.time + 2f;
        }

        private IEnumerator HidePreview(float period)
        {
            while (true)
            {
                if (Time.time >= hideTime)
                {
                    foreach (var image in previewImages)
                    {
                        image.enabled = false;
                    }
                }
                yield return new WaitForSeconds(period);
            }
        }
    }
}
