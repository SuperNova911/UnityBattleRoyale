using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityPUBG.Scripts.Items;

namespace UnityPUBG.Scripts
{
    public class InventoryViewer : MonoBehaviour
    {
        public ItemContainer inventory;
        public Text displayText;

        private StringBuilder stringBuilder = new StringBuilder();

        void Update()
        {
            stringBuilder.Clear();
            foreach (var item in inventory.container)
            {
                stringBuilder.AppendLine($"{item.Data.ItemName}: Stack[{item.CurrentStack}/{item.Data.MaximumStack}], ID[{item.GetHashCode()}]");
            }
            displayText.text = stringBuilder.ToString();
        }
    }
}