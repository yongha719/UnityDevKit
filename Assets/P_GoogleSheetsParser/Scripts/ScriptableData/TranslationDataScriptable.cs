using System;
using System.Collections.Generic;
using UnityEngine;


namespace MyGoogleSheetsParser
{
    public enum LanguageType
    {
        Ko,
        En,
        Ja,
        Cn
    }

    [Serializable]
    public class TranslationData : IGoogleSheetData
    {
        public string Index;

        // List로 들고 Enum으로 index 관리
        public List<string> translatedLanguageList = new List<string>();

        int IGoogleSheetData.Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void InitializeData(string[] rows)
        {
            Index = rows[0];

            translatedLanguageList.Add(rows[1]); // Ko
            translatedLanguageList.Add(rows[2]); // En
            translatedLanguageList.Add(rows[3]); // Ja
            translatedLanguageList.Add(rows[4]); // Cn
        }
    }

    [CreateAssetMenu(fileName = "TranslationDataScriptable", menuName = "MyScriptable/TranslationData", order = 0)]
    public class TranslationDataScriptable : ScriptableObject
    {
        public List<TranslationData> Translations;
    }
}
