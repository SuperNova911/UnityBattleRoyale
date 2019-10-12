using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    [CreateAssetMenu(menuName = "UnityPUBG/ItemData/Backpack")]
    public class BackpackData : ItemData
    {
        #region 필드
        [SerializeField, Range(0, 6)] private int bonusCapacity = 2;
        #endregion

        #region 속성
        public int BonusCapacity => bonusCapacity;
        #endregion

        #region 메서드
        public override Item BuildItem()
        {
            return new ItemBackpack(this);
        }
        #endregion
    }
}
