using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityPUBG.Scripts.Entities;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Logic
{
    public class EntityManager : Singleton<EntityManager>
    {
        private Player myPlayer = null;
        private List<Entity> entities = new List<Entity>();
        private List<Player> players = new List<Player>();
        private List<IDamageable> damageables = new List<IDamageable>();

        public event EventHandler OnMyPlayerSpawn;
        public event EventHandler OnMyPlayerDestory;
        public event EventHandler<Player> OnPlayerSpawn;
        public event EventHandler<Player> OnPlayerDestroy;

        public Player MyPlayer
        {
            get { return myPlayer; }
            set
            {
                myPlayer = value;
                if (myPlayer != null)
                {
                    OnMyPlayerSpawn?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    OnMyPlayerDestory?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public List<Entity> Entities => entities;
        public List<Player> Players => players;
        public List<IDamageable> Damageables => damageables;

        public void RegisterEntity(Entity entity)
        {
            if (entity == null)
            {
                Debug.LogError($"등록하려는 {nameof(entity)}가 null입니다");
                return;
            }

            Entities.Add(entity);
            if (entity is IDamageable)
            {
                Damageables.Add((IDamageable)entity);
            }
            if (entity is Player)
            {
                Players.Add((Player)entity);
                OnPlayerSpawn?.Invoke(this, (Player)entity);
            }
        }

        public void UnRegisterEntity(Entity entity)
        {
            if (entity == null)
            {
                Debug.LogError($"제거하려는 {nameof(entity)}가 null입니다");
                return;
            }

            Entities.Remove(entity);
            if (entity is IDamageable)
            {
                Damageables.Remove((IDamageable)entity);
            }
            if (entity is Player)
            {
                Players.Remove((Player)entity);
                OnPlayerDestroy?.Invoke(this, (Player)entity);
            }
        }
    }
}
