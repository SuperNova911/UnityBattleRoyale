using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Entities
{
    public interface IDamageable
    {
        void OnTakeDamage(float damage, DamageType type);
    }
}
