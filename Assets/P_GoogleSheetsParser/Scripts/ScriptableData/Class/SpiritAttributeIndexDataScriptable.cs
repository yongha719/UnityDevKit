using System;
using UnityEngine;
using MyGoogleSheetsParser;
using System.Collections.Generic;

namespace MyGoogleSheetsParser
{
	[Serializable]
	public class SpiritAttributeIndexData : IGoogleSheetData	
	{
		public int Index;
		public string AttributeName;

        int IGoogleSheetData.Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void InitializeData(string[] rows)
		{
			Index = int.Parse(rows[0]);
			AttributeName = rows[1];
		}
	}
	
	[CreateAssetMenu(fileName = "SpiritAttributeIndexDataScriptable", menuName = "MyScriptable/SpiritAttributeIndexData", order = 1)]
	public class SpiritAttributeIndexDataScriptable : ScriptableObject
	{
		public List<SpiritAttributeIndexData> SpiritAttributeIndexDatas = null;
	}
}
