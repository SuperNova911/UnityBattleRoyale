using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.UI;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts
{
    public class TargetDummy : Entity, IDamageable
    {
        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                OnTakeDamage(UnityEngine.Random.Range(0f, 100f), DamageType.Normal);
            }
        }

        #region IDamageable 인터페이스
        public void OnTakeDamage(float damage, DamageType type)
        {
            FloatingTextDrawer.Instance.DrawDamageText(transform, damage);
        } 
        #endregion
    }
}
