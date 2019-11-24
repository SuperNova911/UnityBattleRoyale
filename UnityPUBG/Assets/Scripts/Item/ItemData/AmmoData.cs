using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/ItemData/Ammo")]
    public class AmmoData : ItemData
    {
        [Header("Ammo Settings")]
        [SerializeField, Range(1f, 100f)] private float projectileSpeed;
        [SerializeField, Range(1f, 30f)] private float range;
        [SerializeField, Range(0.01f, 1f)] private float colliderRadius;

        public float ProjectileSpeed => projectileSpeed;
        public float Range => range;
        public float ColliderRadius => colliderRadius;
    }
}
