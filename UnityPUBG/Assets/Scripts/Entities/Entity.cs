using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        protected int maximumHealth;
        protected float currentHealth;
        protected float movementSpeed;
        protected ItemContainer itemContainer;

        protected void Move(Vector2 direction)
        {
            // move
        }
    }
}