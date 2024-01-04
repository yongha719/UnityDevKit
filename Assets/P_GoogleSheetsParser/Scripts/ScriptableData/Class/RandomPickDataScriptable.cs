using System;
using UnityEngine;
using MyGoogleSheetsParser;
using System.Collections.Generic;

namespace MyGoogleSheetsParser
{
    [Serializable]
    public class RandomPickData : IGoogleSheetData
    {
        public int Index;
        public string Title;
        public int GroupID;
        public int Weight;

        int IGoogleSheetData.Index { get; set; }

        public void InitializeData(string[] rows)
        {
            Index = int.Parse(rows[0]);
            Title = rows[1];
            GroupID = int.Parse(rows[2]);
            Weight = int.Parse(rows[3]);
        }
    }

    [CreateAssetMenu(fileName = "RandomPickDataScriptable", menuName = "MyScriptable/RandomPickData", order = 1)]
    public class RandomPickDataScriptable : ScriptableObject
    {
        public List<RandomPickData> RandomPickDatas = null;
    }
}
