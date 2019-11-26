using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Knife.PostProcessing
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Renderer))]
    public class OutlineRegister : MonoBehaviour
    {
        public Color OutlineTint = new Color(1, 1, 1, 1);
        private Renderer cachedRenderer;

        public Renderer CachedRenderer
        {
            get
            {
                if (cachedRenderer == null)
                    cachedRenderer = GetComponent<Renderer>();

                return cachedRenderer;
            }
        }

        void OnEnable()
        {
            OutlineRenderer.AddRenderer(CachedRenderer);
            SetupPropertyBlock();
        }

        private void OnValidate()
        {
            SetupPropertyBlock();
        }

        void OnDisable()
        {
            OutlineRenderer.RemoveRenderer(CachedRenderer);
        }

        public void SetTintColor(Color color)
        {
            OutlineTint = color;
            SetupPropertyBlock();
        }

        private void SetupPropertyBlock()
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            CachedRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("_OutlineColor", OutlineTint);
            CachedRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}