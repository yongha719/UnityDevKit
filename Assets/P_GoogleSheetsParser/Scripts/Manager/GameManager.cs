using MyInfinityScrollUI;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGoogleSheetsParser
{
    public class GameManager : Singleton<GameManager>
    {
        protected override bool dontDestroy => true;

        public LanguageType TranslationType;

        /// <summary>
        /// 언어가 바뀌었을 때 Text 업데이트
        /// </summary>
        [Button]
        public void UpdateTranslationText()
        {
            SceneManager.LoadScene(0);
        }
    }
}
