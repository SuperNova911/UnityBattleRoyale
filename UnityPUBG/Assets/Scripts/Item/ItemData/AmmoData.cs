using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/ItemData/Ammo")]
    public class AmmoData : UsableItemData
    {
        #region 메서드
        public override Item BuildItem()
        {
            return new ItemAmmo(this);
        }
        #endregion
    }
}
