using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Logic;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.UI
{
    public class FloatingTextDrawer : Singleton<FloatingTextDrawer>
    {
        public Canvas canvas;

        public DamageText damageTextPrefab;

        private void Awake()
        {
            if (canvas == null)
            {
                canvas = FindObjectOfType<Canvas>();
            }

            ObjectPoolManager.Instance.InitializeUIObjectPool(damageTextPrefab.gameObject, canvas, 10);
        }

        public void DrawDamageText(Transform damageReceiver, float damage)
        {
            var damageTextPoolObject = ObjectPoolManager.Instance.ReuseUIObject(damageTextPrefab.gameObject);
            if (damageTextPoolObject == null)
            {
                return;
            }

            var damageText = damageTextPoolObject.GetComponent<DamageText>();
            damageText.DamageReceiver = damageReceiver;
            damageText.text.text = Mathf.RoundToInt(damage).ToString();
        }
    }
}
