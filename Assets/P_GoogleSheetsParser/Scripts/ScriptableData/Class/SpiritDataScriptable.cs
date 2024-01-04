using System;
using UnityEngine;
using MyGoogleSheetsParser;
using System.Collections.Generic;

namespace MyGoogleSheetsParser
{
	[Serializable]
    public class SpiritData : IGoogleSheetData
    {
		public string Name;
		public int Count;
		public int Level;
		public int[] AttributeIndex;

        int IGoogleSheetData.Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void InitializeData(string[] rows)
		{
			Name = rows[0];
			Count = int.Parse(rows[1]);
			Level = int.Parse(rows[2]);
			AttributeIndex = Array.ConvertAll(rows[3].Split(','), int.Parse);
		}
	}
	
	[CreateAssetMenu(fileName = "SpiritDataScriptable", menuName = "MyScriptable/SpiritData", order = 1)]
	public class SpiritDataScriptable : ScriptableObject
	{
		public List<SpiritData> SpiritDatas = null;
	}
}
