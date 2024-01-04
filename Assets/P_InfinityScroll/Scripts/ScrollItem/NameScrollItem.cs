using TMPro;
using UnityEngine;

namespace MyInfinityScrollUI
{
    /// <summary>
    /// -<see cref="InfinityScroll"/>
    /// <br></br><br></br>
    /// <see cref="ContentItem"/>를 상속받음<br></br>
    /// <see cref="NameItemData"/>를 사용
    /// </summary>
    public class NameScrollItem : ScrollItemBase
    {
        [Header(nameof(NameScrollItem))]

        [SerializeField]
        protected TextMeshProUGUI countText;

        [SerializeField]
        protected TextMeshProUGUI nameText;

        // ContentItem에 있는 contentItemData를 형변환시켜서 가져옴
        public NameItemData ColorAndNameData;

        public override void InitializeItemData(object itemData)
        {
            ColorAndNameData = (NameItemData)itemData;
            base.InitializeItemData(itemData);
        }

        public override void UpdateData()
        {
            base.UpdateData();

            countText.text = ColorAndNameData.Count.ToString();
            nameText.text = ColorAndNameData.Name;
            nameText.color = ColorAndNameData.NameTextColor;
        }
    }
}