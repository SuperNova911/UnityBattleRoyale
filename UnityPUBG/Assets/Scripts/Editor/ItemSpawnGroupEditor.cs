using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityPUBG.Scripts.Logic;

namespace UnityPUBG.Scripts
{
    [CustomEditor(typeof(ItemSpawnGroup))]
    public class ItemSpawnGroupEditor : Editor
    {
        ItemSpawnGroup itemSpawnGroup;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Apply to ItemSpawnPoints"))
            {
                itemSpawnGroup.ApplyToItemSpawnPoints();
            }
        }

        private void OnEnable()
        {
            itemSpawnGroup = (ItemSpawnGroup)target;
        }
    }
}
