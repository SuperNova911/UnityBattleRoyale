using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Knife.PostProcessing
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OutlineRegister))]
    public class OutlineRegisterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Label(OutlineEditor.LogoTexture);
            base.OnInspectorGUI();
        }
    }
}