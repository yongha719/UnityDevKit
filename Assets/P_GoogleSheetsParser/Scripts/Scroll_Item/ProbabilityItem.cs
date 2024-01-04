using MyInfinityScrollUI;
using TMPro;
using UnityEngine;

namespace MyGoogleSheetsParser
{
    public class ProbabilityItem : ScrollItemBase
    {
        [SerializeField]
        private TextMeshProUGUI titleText;

        [SerializeField]
        private TextMeshProUGUI probabilityText;

        public ProbabilityData ProbabilityData;

        public override void InitializeItemData(object itemData)
        {
            ProbabilityData = (ProbabilityData)itemData;

            base.InitializeItemData(itemData);
        }

        public override void UpdateData()
        {
            base.UpdateData();

            // titleText.text = ProbabilityData.Title; 기존 방식
            titleText.text = ResourceManager.Instance.GetTranslatedLanguage(ProbabilityData.Title);
            probabilityText.text = $"{ProbabilityData.Probability:0.000}%";
        }
    }
}
