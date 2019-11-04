using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts
{
    public class FloatingTextDrawer : Singleton<FloatingTextDrawer>
    {
        public Canvas canvas;

        public DamageText damageTextObject;

        public void DrawDamageText(Transform damageReceiver, float damage)
        {
            var damageText = Instantiate(damageTextObject, canvas.transform);
            damageText.damageReceiver = damageReceiver;

            damageText.text.text = Mathf.RoundToInt(damage).ToString();
        }
    }
}
