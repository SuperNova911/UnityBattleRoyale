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

        private void Awake()
        {
            enabled = false;
        }

        private void OnBecameVisible()
        {
            enabled = true;
        }

        private void OnBecameInvisible()
        {
            enabled = false;
        }

        private void OnEnable()
        {
            OutlineRenderer.AddRenderer(CachedRenderer);
            SetupPropertyBlock();
        }

        private void OnDisable()
        {
            OutlineRenderer.RemoveRenderer(CachedRenderer);
        }

        private void OnValidate()
        {
            SetupPropertyBlock();
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