using UnityPUBG.Scripts.Items;
using UnityPUBG.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityPUBG.Scripts.Entities
{
    public class Player : Entity
    {
        #region 필드
        private InputManager inputManager; 
        #endregion

        #region 속성
        public WeaponData EquipedWeapon { get; private set; }
        public ArmorData EquipedArmor { get; private set; }
        public BackpackData EquipedBackpack { get; private set; }
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

            ControlMovement();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
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
        /// <summary>
        /// InputSystem으로부터 입력받은 값을 기반으로 Player의 움직임을 컨트롤
        /// </summary>
        private void ControlMovement()
        {
            Vector2 direction = inputManager.Player.Movement.ReadValue<Vector2>();
            movementDirection = new Vector3(direction.x, 0, direction.y);
        } 
        #endregion
    }
}
