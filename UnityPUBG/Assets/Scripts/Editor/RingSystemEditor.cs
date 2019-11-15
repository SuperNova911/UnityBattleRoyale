using UnityEditor;
using UnityEngine;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts
{
    [CustomEditor(typeof(RingSystem))]
    public class RingSystemEditor : Editor
    {
        RingSystem ringSystem;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Round Datas"))
            {
                ringSystem.GenerateRoundDatas();
            }
        }

        private void OnEnable()
        {
            ringSystem = (RingSystem)target;
        }
    }
}
