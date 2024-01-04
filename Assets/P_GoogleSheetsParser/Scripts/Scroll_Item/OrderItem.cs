using MyInfinityScrollUI;
using TMPro;
using UnityEngine;

namespace MyGoogleSheetsParser
{
    public class OrderItem : ScrollItemBase
    {
        [SerializeField]
        private TextMeshProUGUI ordererNameText;
        [SerializeField]
        private TextMeshProUGUI characterNameText;

        public OrderData orderItemData;

        public override void InitializeItemData(object itemData)
        {
            orderItemData = (OrderData)itemData;
            base.InitializeItemData(itemData);
        }

        public override void UpdateData()
        {
            base.UpdateData();

            ordererNameText.text = orderItemData.OrdererName;
            characterNameText.text = orderItemData.CharacterName;
        }
    }
}
