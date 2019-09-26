using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        #region 필드
        [Header("Health")]
        [SerializeField] protected int maximumHealth;
        [SerializeField] protected float currentHealth;

        [Header("Movement")]
        [SerializeField] protected float movementSpeed;
        [SerializeField] private Vector3 currentVelocity;
        [SerializeField] private float smoothTime;

        protected ItemContainer itemContainer;

        private Rigidbody entityRigidbody;
        #endregion

        #region 유니티 메시지
        protected virtual void Awake()
        {
            currentHealth = maximumHealth;
            entityRigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {

        }
        #endregion

        #region 메서드
        protected void Move(Vector3 direction)
        {
            Vector3 targetVelocity = new Vector3(direction.x * movementSpeed, entityRigidbody.velocity.y, direction.z * movementSpeed);
            entityRigidbody.velocity = Vector3.SmoothDamp(entityRigidbody.velocity, targetVelocity, ref currentVelocity, smoothTime);
        } 
        #endregion
    }
}