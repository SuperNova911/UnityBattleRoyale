using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UnityPUBG.Scripts.Utilities
{
    public class JoystickResizer : MonoBehaviour
    {
        public RectTransform background;
        public RectTransform handle;

        [Header("Settings")]
        [Range(0.1f, 1f)]
        public float backgroundRatio = 0.3f;
        [Range(0.1f, 1f)]
        public float handleRatio = 0.5f;

        [Header("Debug")]
        public bool previewChange = false;

        private Image backgroundImage;
        private Image handleImage;

        private void Awake()
        {
            backgroundImage = background.gameObject.GetComponent<Image>();
            handleImage = background.gameObject.GetComponent<Image>();

            Resize();
        }

        private void OnValidate()
        {
            if (Application.isPlaying == false && previewChange)
            {
                if (backgroundImage == null || handleImage == null)
                {
                    backgroundImage = background.gameObject.GetComponent<Image>();
                    handleImage = background.gameObject.GetComponent<Image>();
                }

                Resize();
            }
        }

        private void Resize()
        {
            int screenHeight = Screen.height;
            float backgroundSpriteHeight = backgroundImage.sprite.rect.height;
            float handleSpriteHeight = handleImage.sprite.rect.height;

            float screenRatio = screenHeight / backgroundSpriteHeight;

            background.sizeDelta = new Vector2(backgroundSpriteHeight * screenRatio * backgroundRatio, backgroundSpriteHeight * screenRatio * backgroundRatio);
            handle.sizeDelta = new Vector2(handleSpriteHeight * screenRatio * backgroundRatio * handleRatio, handleSpriteHeight * screenRatio * backgroundRatio * handleRatio);
        }
    }
}
