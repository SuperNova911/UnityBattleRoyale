using Assets.Scripts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities
{
    public class Player : Entity
    {
        public Weapon EquipedWeapon { get; private set; }
        public Armor EquipedArmor { get; private set; }
        public Backpack EquipedBackpack { get; private set; }
        public Vehicle RidingVehicle { get; private set; }
    }
}
