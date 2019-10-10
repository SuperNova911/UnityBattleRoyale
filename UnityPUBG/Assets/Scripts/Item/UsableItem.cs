using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityPUBG.Scripts.Items
{
    public abstract class UsableItem : Item
    {
        [SerializeField] private int maximunStack = 6;
        [SerializeField] private int currentStack = 0;

        public int MaximumStack => maximunStack;
        public int CurrentStack => currentStack;
    }
}
