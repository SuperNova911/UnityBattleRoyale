using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Logic;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts
{
    [RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
    public class ProjectileBase : PoolObject
    {
        public float speed;
        public float lifeTime;
        public float damage;
        public DamageType damageType;

        private Vector3 fireDirection;
        private bool isFired = false;

        private SphereCollider projectileSphereCollider;
        private Rigidbody projectileRigidbody;

        #region 유니티 메시지
        private void Awake()
        {
            projectileSphereCollider = GetComponent<SphereCollider>();
            projectileRigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (isFired)
            {
                projectileRigidbody.MovePosition(transform.position + fireDirection * speed * Time.fixedDeltaTime);
            }
        }

        private void OnDestroy()
        {
        }
        #endregion

        #region PoolObject
        public override void OnObjectReuse()
        {

        }

        public override void OnObjectSaveToPool()
        {

        }
        #endregion

        public void InitializeProjectile(string ammoDataName)
        {
            if (ItemDataCollection.Instance.ItemDataByName.TryGetValue(ammoDataName, out var itemData))
            {
                if (itemData is AmmoData == false)
                {
                    Debug.LogWarning($"");
                    return;
                }

                var ammoData = itemData as AmmoData;
                speed = ammoData.ProjectileSpeed;
                //lifeTime = ammoData.Range / speed;
                projectileSphereCollider.radius = ammoData.ColliderRadius;
            }
            else
            {
                Debug.LogWarning($"");
            }
        }

        public void Fire(Vector3 direction)
        {
            fireDirection = direction;
            isFired = true;
            Destroy(gameObject, lifeTime);
        }
    }
}
