using Assets.Scripts.Items;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Entities
{
    public class Player : Entity
    {
        #region 필드
        private InputManager inputManager; 
        #endregion

        #region 속성
        public Weapon EquipedWeapon { get; private set; }
        public Armor EquipedArmor { get; private set; }
        public Backpack EquipedBackpack { get; private set; }
        public Vehicle RidingVehicle { get; private set; }
        #endregion

        #region 유니티 메시지
        protected override void Awake()
        {
            base.Awake();

            inputManager = new InputManager();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            ControlMovement();
        }

        private void OnEnable()
        {
            inputManager.Enable();
        }

        private void OnDisable()
        {
            inputManager.Disable();
        }
        #endregion

        #region 메서드
        private void ControlMovement()
        {
            Vector2 direction = inputManager.Player.Movement.ReadValue<Vector2>();
            Move(direction.x, direction.y);
        } 
        #endregion
    }
}
