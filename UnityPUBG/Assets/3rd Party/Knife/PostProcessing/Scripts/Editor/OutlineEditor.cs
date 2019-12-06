using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER && !UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Knife.PostProcessing
{
    [PostProcessEditor(typeof(Outline))]
    public class OutlineEditor : PostProcessEffectEditor<Outline>
    {
        SerializedParameterOverride outlineGlobalIntensity;
        SerializedParameterOverride outlineThickness;
        SerializedParameterOverride outlineFillAmount;
        SerializedParameterOverride depthOverlap;
        SerializedParameterOverride depthCulling;
        SerializedParameterOverride depthInvert;
        SerializedParameterOverride depthOutlineBreak;
        SerializedParameterOverride cornerOutlines;

        SerializedParameterOverride blurOutline;
        SerializedParameterOverride blurScale;
        SerializedParameterOverride iterations;
        SerializedParameterOverride downsampleSize;
        SerializedParameterOverride overGlow;
        SerializedParameterOverride hardness;

        static Texture2D logoTexture;

        public static Texture2D LogoTexture
        {
            get
            {
                if (logoTexture == null)
                    logoTexture = Resources.Load<Texture2D>("Knife-Outline/LOGO");

                return logoTexture;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            outlineGlobalIntensity = FindParameterOverride(x => x.OutlineGlobalIntensity);
            outlineThickness = FindParameterOverride(x => x.OutlineThickness);
            outlineFillAmount = FindParameterOverride(x => x.OutlineFillAmount);
            depthOverlap = FindParameterOverride(x => x.DepthOverlap);
            depthCulling = FindParameterOverride(x => x.DepthCulling);
            depthInvert = FindParameterOverride(x => x.InvertDepthCulling);
            depthOutlineBreak = FindParameterOverride(x => x.DepthOutlineBreak);
            cornerOutlines = FindParameterOverride(x => x.CornerOutlines);

            blurOutline = FindParameterOverride(x => x.BlurOutline);
            blurScale = FindParameterOverride(x => x.BlurScale);
            iterations = FindParameterOverride(x => x.Iterations);
            downsampleSize = FindParameterOverride(x => x.DownsampleSize);
            overGlow = FindParameterOverride(x => x.OverGlow);
            hardness = FindParameterOverride(x => x.Hardness);
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label(LogoTexture);
            EditorUtilities.DrawHeaderLabel("Outline");

            PropertyField(outlineGlobalIntensity);
            PropertyField(outlineThickness);
            PropertyField(outlineFillAmount);
            PropertyField(depthCulling);
            
            if (depthCulling.value.boolValue)
            {
                PropertyField(depthOverlap);
                PropertyField(depthOutlineBreak);
                PropertyField(depthInvert);
            }
            PropertyField(cornerOutlines);

            EditorUtilities.DrawHeaderLabel("Blur");

            PropertyField(blurOutline);

            if (blurOutline.value.boolValue)
            {
                PropertyField(blurScale);
                PropertyField(iterations);
                PropertyField(downsampleSize);
                PropertyField(overGlow);
                PropertyField(hardness);
            }

            EditorGUILayout.Space();
        }
    }
}