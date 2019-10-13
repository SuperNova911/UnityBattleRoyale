using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/LootAnimationSettings")]
    public class LootAnimationSettings : ScriptableObject
    {
        #region 필드
        [Header("Phase 1")]
        [SerializeField, Range(1f, 20f)] private float phase1Speed = 8f;
        [SerializeField] private Vector3 floatingOffset = new Vector3(0, 2.5f, 0.5f);
        [SerializeField] private Vector3 randomRange = new Vector3(1f, 0, 0);

        [Header("Phase 2")]
        [SerializeField, Range(1f, 20f)] private float phase2Speed = 3f;

        [Header("Phase 3")]
        [SerializeField, Range(1f, 20f)] private float phase3Speed = 10f;
        [SerializeField] private Vector3 endOffset = new Vector3(0, 1.5f, 0);
        [SerializeField] private Vector3 endScale = new Vector3(0, 0, 0);
        #endregion

        #region 속성
        public float Phase1Speed => phase1Speed;
        public Vector3 FloatingOffset => floatingOffset;
        public Vector3 RandomRange => randomRange;

        public float Phase2Speed => phase2Speed;

        public float Phase3Speed => phase3Speed;
        public Vector3 EndOffset => endOffset;
        public Vector3 EndScale => endScale;
        #endregion
    }

}
