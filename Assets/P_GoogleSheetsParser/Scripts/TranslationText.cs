using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MyGoogleSheetsParser
{
    public class TranslationText : MonoBehaviour
    {
        [SerializeField]
        private string index;

        private TextMeshProUGUI translationText;

        private string originText;

        // 기존 Text 컴포넌트처럼 사용가능
        public string text
        {
            set
            {
                translationText.text = value;
            }
        }

        private void Awake()
        {
            translationText = GetComponent<TextMeshProUGUI>();
        }

        void OnEnable()
        {
            // 값이 바뀌어도 돌아올 수 있게 저장해놓음
            originText = ResourceManager.Instance.GetTranslatedLanguage(index);
            translationText.text = originText;
        }

        // 값이 바뀌었다가 다시 원래 값으로 초기화되어야 하는 Text의 경우
        public void SetOriginText()
        {
            translationText.text = originText;
        }
    }
}