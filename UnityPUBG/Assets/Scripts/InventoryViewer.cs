using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

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
                stringBuilder.AppendLine($"{item.ItemName}: Stack[{item.CurrentStack}/{item.MaximumStack}], ID[{item.GetInstanceID()}]");
            }
            displayText.text = stringBuilder.ToString();
        }
    }
}