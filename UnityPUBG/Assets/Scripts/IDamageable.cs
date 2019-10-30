using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityPUBG.Scripts
{
    public enum DamageType
    {
        Normal, Explosive, Absolute
    }

    public interface IDamageable
    {
        void OnTakeDamage(float damage, DamageType type);
    }
}
