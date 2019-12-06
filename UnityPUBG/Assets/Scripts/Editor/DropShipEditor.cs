using UnityEditor;
using UnityEngine;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts
{
    [CustomEditor(typeof(DropShip))]
    public class DropShipEditor : Editor
    {
        DropShip dropShip;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Path"))
            {
                dropShip.GeneratePath();
            }
        }

        private void OnEnable()
        {
            dropShip = (DropShip)target;
        }
    }
}
