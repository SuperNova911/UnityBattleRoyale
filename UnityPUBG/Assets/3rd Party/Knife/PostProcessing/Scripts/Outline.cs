using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using BoolParameter = UnityEngine.Rendering.PostProcessing.BoolParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using MinAttribute = UnityEngine.Rendering.PostProcessing.MinAttribute;
using Object = UnityEngine.Object;
using Vector2Parameter = UnityEngine.Rendering.PostProcessing.Vector2Parameter;

namespace Knife.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(OutlineRenderer), PostProcessEvent.AfterStack, "Knife/Outline")]
    // Uncomment next line if you want that other post process effects works with outline (like bloom, depth of field or motion blur etc.)
    // and comment previous line
    // [PostProcess(typeof(OutlineRenderer), PostProcessEvent.BeforeStack, "Knife/Outline")]
    // if you will use BeforeStack event, so you can not use Forward Rendering in Built-in Unity Render Pipeline
    public class Outline : PostProcessEffectSettings
    {
        [Range(0f, 1)]
        [Tooltip("Outline alpha")]
        public FloatParameter OutlineGlobalIntensity = new FloatParameter() { value = 1f };
        [Tooltip("Lines thickness")]
        [Min(1)]
        public IntParameter OutlineThickness = new IntParameter() { value = 4 };
        [Range(0f, 1f)]
        [Tooltip("Object color fill amount")]
        public FloatParameter OutlineFillAmount = new FloatParameter() { value = 0f };
        [Range(0f, 5000f)]
        [Tooltip("Depth threshold")]
        public FloatParameter DepthOverlap = new FloatParameter() { value = 1f };
        [Tooltip("Cull outline by depth (GPU)")]
        public BoolParameter DepthCulling = new BoolParameter() { value = true };
        [Tooltip("Invert outline culling by depth (draw not visible part of mesh if enabled)")]
        public BoolParameter InvertDepthCulling = new BoolParameter() { value = false };
        [Tooltip("Break outline in intersections if enabled")]
        public BoolParameter DepthOutlineBreak = new BoolParameter() { value = false };
        [Tooltip("Additional sampling for corner outlines")]
        public BoolParameter CornerOutlines = new BoolParameter() { value = true };

        [Tooltip("Blur outlines after depth culling")]
        public BoolParameter BlurOutline = new BoolParameter() { value = false };
        [Tooltip("Blur step amount")]
        public Vector2Parameter BlurScale = new Vector2Parameter() { value = new Vector2(2, 2) };
        [Tooltip("Blur iterations count")]
        public IntParameter Iterations = new IntParameter() { value = 12 };
        [Range(32, 2048)]
        [Tooltip("Blur textures downsampling size")]
        public IntParameter DownsampleSize = new IntParameter() { value = 1024 };
        [Range(0f, 10f)]
        [Tooltip("Overglow outline after blur")]
        public FloatParameter OverGlow = new FloatParameter() { value = 3f };
        [Range(0f, 1f)]
        [Tooltip("Hardness outline after blur")]
        public FloatParameter Hardness = new FloatParameter() { value = 0f };
    }

    public class OutlineRenderer : PostProcessEffectRenderer<Outline>
    {
        static Shader outlineShader;
        static Shader outlineBufferShader;
        static Material outlineMaterial;

        static Material OutlineMaterial
        {
            get
            {
                if (outlineMaterial == null)
                {
                    outlineMaterial = new Material(outlineShader);
                    outlineMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return outlineMaterial;
            }
        }

        static Material OutlineBufferMaterial
        {
            get
            {
                if(outlineBufferMaterial == null)
                {
                    outlineBufferMaterial = new Material(outlineBufferShader);
                    outlineBufferMaterial.hideFlags = HideFlags.HideAndDontSave;
                }

                return outlineBufferMaterial;
            }
        }

        static Material outlineBufferMaterial;

        static List<Renderer> renderers = new List<Renderer>();
        static Action onTargetsChanged;

        Dictionary<Camera, CameraOutlineData> registeredCameras = new Dictionary<Camera, CameraOutlineData>();

        Shader blurShader;
        Material blurMaterial;

        Material BlurMaterial
        {
            get
            {
                if (blurMaterial == null)
                {
                    blurMaterial = new Material(blurShader);
                    blurMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return blurMaterial;
            }
        }

        int uScaleProp = -1;

        int lineThicknessXProp = -1;
        int lineThicknessYProp = -1;
        int fillAmountProp = -1;
        int outlineSourceProp = -1;
        int outlineGlobalIntensityProp = -1;
        int outlineDepthProp = -1;
        int drawOnlyVisiblePartProp = -1;
        int depthDiffSignProp = -1;
        int depthOverlapProp = -1;
        int overglowProp = -1;
        int hardnessProp = -1;

        public override void Init()
        {
            base.Init();

            if (blurMaterial != null)
                Object.DestroyImmediate(blurMaterial, true);

            if (outlineMaterial != null)
                Object.DestroyImmediate(outlineMaterial, true);

            if (outlineBufferMaterial != null)
                Object.DestroyImmediate(outlineBufferMaterial, true);

            outlineShader = Shader.Find("Knife/Post Processing/Outline Effect");
            outlineBufferShader = Shader.Find("Knife/Post Processing/Outline");

            blurShader = Shader.Find("Knife/Post Processing/Gaussian Blur");

            uScaleProp = Shader.PropertyToID("_u_Scale");

            lineThicknessXProp = Shader.PropertyToID("_LineThicknessX");
            lineThicknessYProp = Shader.PropertyToID("_LineThicknessY");
            fillAmountProp = Shader.PropertyToID("_FillAmount");
            outlineSourceProp = Shader.PropertyToID("_OutlineSource");
            outlineGlobalIntensityProp = Shader.PropertyToID("_OutlineGlobalIntensity");

            outlineDepthProp = Shader.PropertyToID("_OutlineDepth");
            drawOnlyVisiblePartProp = Shader.PropertyToID("_DrawOnlyVisiblePart");
            depthDiffSignProp = Shader.PropertyToID("_DepthDiffSign");
            depthOverlapProp = Shader.PropertyToID("_DepthOverlap");

            overglowProp = Shader.PropertyToID("_overGlow");
            hardnessProp = Shader.PropertyToID("_hardness");
        }

        public static void AddRenderer(Renderer r)
        {
            if(!renderers.Contains(r))
                renderers.Add(r);

            if(onTargetsChanged != null)
            {
                onTargetsChanged();
            }
        }

        public static void RemoveRenderer(Renderer r)
        {
            renderers.Remove(r);

            onTargetsChanged?.Invoke();
        }

        public override void Render(PostProcessRenderContext context)
        {
            CameraOutlineData renderData;
            if(!registeredCameras.TryGetValue(context.camera, out renderData))
            {
                renderData = new CameraOutlineData(context.camera);
                registeredCameras.Add(context.camera, renderData);
            }

            var cmd = context.command;

            renderData.SetupBuffer();
            renderData.Render();

            float lineThinkness = settings.OutlineThickness.value;
            if(context.isSceneView && settings.OutlineThickness.value > 1)
            {
                lineThinkness = settings.OutlineThickness.value * 0.5f;
                lineThinkness = Mathf.Round(lineThinkness);
            }

            OutlineMaterial.SetFloat(lineThicknessXProp, (lineThinkness / 1000.0f) * (1.0f / Screen.width) * 1000.0f);
            OutlineMaterial.SetFloat(lineThicknessYProp, (lineThinkness / 1000.0f) * (1.0f / Screen.height) * 1000.0f);
            OutlineMaterial.SetFloat(fillAmountProp, settings.OutlineFillAmount.value);
            OutlineMaterial.SetTexture(outlineSourceProp, renderData.ColorTexture);
            OutlineMaterial.SetFloat(outlineGlobalIntensityProp, settings.OutlineGlobalIntensity.value);

            if (settings.DepthCulling)
                OutlineMaterial.EnableKeyword("DEPTH_CULLING");
            else
                OutlineMaterial.DisableKeyword("DEPTH_CULLING");

            if (settings.CornerOutlines)
                OutlineMaterial.EnableKeyword("CORNER_OUTLINES");
            else
                OutlineMaterial.DisableKeyword("CORNER_OUTLINES");

            if (settings.DepthCulling)
            {
                OutlineMaterial.SetTexture(outlineDepthProp, renderData.DepthTexture);
                OutlineMaterial.SetInt(drawOnlyVisiblePartProp, settings.DepthOutlineBreak.value ? 0 : 1);
                OutlineMaterial.SetInt(depthDiffSignProp, settings.InvertDepthCulling.value ? -1 : 1);
                OutlineBufferMaterial.SetFloat(depthOverlapProp, -settings.DepthOverlap.value);
            }

            if (settings.BlurOutline)
            {
                renderData.SetupBlur(settings.DownsampleSize);

                cmd.BeginSample("Outline blit " + context.camera.name);
                cmd.Blit(context.source, renderData.ExtraColorTexture, OutlineMaterial, 1);
                cmd.EndSample("Outline blit " + context.camera.name);

                Vector2 affect = new Vector2(1.0f / settings.DownsampleSize, 1.0f / settings.DownsampleSize);
                var frac = Vector2.Scale(affect, settings.BlurScale);

                cmd.Blit(renderData.ExtraColorTexture, renderData.BlurTemp1);

                for (int i = 0; i < settings.Iterations; i++)
                {
                    cmd.SetGlobalVector(uScaleProp, new Vector2(frac.x, 0f));
                    cmd.Blit(renderData.BlurTemp1, renderData.BlurTemp2, BlurMaterial, 0);

                    cmd.SetGlobalVector(uScaleProp, new Vector2(0f, frac.y));
                    cmd.Blit(renderData.BlurTemp2, renderData.BlurTemp1, BlurMaterial, 0);
                }

                cmd.Blit(renderData.BlurTemp1, renderData.ExtraColorTexture);
                cmd.Blit(renderData.ExtraColorTexture, renderData.ColorTexture);

                OutlineMaterial.SetFloat(hardnessProp, 1 + settings.Hardness);
                OutlineMaterial.SetFloat(overglowProp, settings.OverGlow);
                OutlineMaterial.SetTexture(outlineSourceProp, renderData.ColorTexture);
                cmd.Blit(context.source, context.destination, OutlineMaterial, 2);
            }
            else
            {
                OutlineMaterial.SetFloat(overglowProp, 1);
                cmd.BeginSample("Outline blit " + context.camera.name);
                cmd.Blit(context.source, context.destination, OutlineMaterial, 0);
                cmd.EndSample("Outline blit " + context.camera.name);
            }

        }

        public class CameraOutlineData
        {
            public CommandBuffer Buffer;
            public RenderTexture ColorTexture;
            public RenderTexture DepthTexture;
            public Camera Camera;
            public RenderTexture BlurTemp1;
            public RenderTexture BlurTemp2;
            public RenderTexture ExtraColorTexture;

            public CameraOutlineData(Camera camera)
            {
                init(camera);
            }

            void init(Camera camera)
            {
                Camera = camera;

                ColorTexture = new RenderTexture(camera.scaledPixelWidth, camera.scaledPixelHeight, 16, RenderTextureFormat.ARGB32);
                ColorTexture.Create();
                DepthTexture = new RenderTexture(camera.scaledPixelWidth, camera.scaledPixelHeight, 16, RenderTextureFormat.Depth);
                DepthTexture.Create();

                Buffer = new CommandBuffer();
                Buffer.name = "Outline render";
            }

            public void SetupBlur(int downsampleSize)
            {
                if (BlurTemp1 == null || BlurTemp1.width != downsampleSize)
                {
                    if (BlurTemp1 != null)
                        RenderTexture.ReleaseTemporary(BlurTemp1);

                    if (BlurTemp2 != null)
                        RenderTexture.ReleaseTemporary(BlurTemp2);

                    BlurTemp1 = RenderTexture.GetTemporary(downsampleSize, downsampleSize);
                    BlurTemp2 = RenderTexture.GetTemporary(downsampleSize / 2, downsampleSize / 2);
                }

                if (ExtraColorTexture == null || ExtraColorTexture.width != Camera.scaledPixelWidth)
                {
                    if(ExtraColorTexture != null)
                        Object.DestroyImmediate(ExtraColorTexture, true);

                    ExtraColorTexture = new RenderTexture(Camera.scaledPixelWidth, Camera.scaledPixelHeight, 16, RenderTextureFormat.ARGB32);
                    ExtraColorTexture.Create();
                }
            }

            public void SetupBuffer()
            {
                if (ColorTexture == null)
                    init(Camera);

                if (ColorTexture.width != Camera.scaledPixelWidth || ColorTexture.height != Camera.scaledPixelHeight)
                {
                    Object.DestroyImmediate(ColorTexture, true);
                    Object.DestroyImmediate(DepthTexture, true);

                    ColorTexture = new RenderTexture(Camera.scaledPixelWidth, Camera.scaledPixelHeight, 16, RenderTextureFormat.ARGB32);
                    ColorTexture.Create();
                    DepthTexture = new RenderTexture(Camera.scaledPixelWidth, Camera.scaledPixelHeight, 16, RenderTextureFormat.Depth);
                    DepthTexture.Create();
                }

                Buffer.Clear();
                if (ExtraColorTexture != null)
                {
                    Buffer.SetRenderTarget(ExtraColorTexture);
                    Buffer.ClearRenderTarget(true, true, Color.clear);
                }
                Buffer.SetRenderTarget(ColorTexture, DepthTexture);
                Buffer.ClearRenderTarget(true, true, Color.clear);
                Buffer.SetProjectionMatrix(Camera.projectionMatrix);
                Buffer.SetViewMatrix(Camera.worldToCameraMatrix);

                foreach (var r in renderers)
                {
                    MeshFilter mf = r.GetComponent<MeshFilter>();
                    SkinnedMeshRenderer smr = r as SkinnedMeshRenderer;
                    if (mf != null)
                    {
                        for (int i = 0; i < mf.sharedMesh.subMeshCount; i++)
                        {
                            Buffer.DrawRenderer(r, OutlineBufferMaterial, i, 0);
                        }
                    }
                    else if (smr != null)
                    {
                        for (int i = 0; i < smr.sharedMesh.subMeshCount; i++)
                        {
                            Buffer.DrawRenderer(r, OutlineBufferMaterial, i, 0);
                        }
                    }
                    else
                    {
                        Buffer.DrawRenderer(r, OutlineBufferMaterial, 0, 0);
                    }
                }
            }

            public void Render()
            {
                //Buffer.BeginSample("Outline render " + Camera.name);
                Graphics.ExecuteCommandBuffer(Buffer);
                //Buffer.EndSample("Outline render " + Camera.name);
            }
        }
    }
}