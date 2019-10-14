using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/ItemData/Armor")]
    public class ArmorData : ItemData
    {
        #region 필드
        [SerializeField, Range(0, 100)] private int shieldAmount = 50;
        #endregion

        #region 속성
        public int ShieldAmount => shieldAmount;
        #endregion

        #region 메서드
        public override Item BuildItem()
        {
            return new ItemArmor(this);
        }
        #endregion
    }
}
