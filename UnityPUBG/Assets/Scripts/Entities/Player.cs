using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class Player : Entity
    {
        #region 속성
        public Weapon EquipedWeapon { get; private set; }
        public Armor EquipedArmor { get; private set; }
        public Backpack EquipedBackpack { get; private set; }
        public Vehicle RidingVehicle { get; private set; }
        #endregion

        #region 유니티 메시지
        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        } 
        #endregion
    }
}
