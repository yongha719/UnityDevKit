using System;
using UnityEngine;
using MyGoogleSheetsParser;
using System.Collections.Generic;

namespace MyGoogleSheetsParser
{
	[Serializable]
	public class OrderData : IGoogleSheetData	
	{
		public int Index;
		public string OrdererName;
		public string CharacterName;

        int IGoogleSheetData.Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void InitializeData(string[] rows)
		{
			Index = int.Parse(rows[0]);
			OrdererName = rows[1];
			CharacterName = rows[2];
		}
	}
	
	[CreateAssetMenu(fileName = "OrderDataScriptable", menuName = "MyScriptable/OrderData", order = 1)]
	public class OrderDataScriptable : ScriptableObject
	{
		public List<OrderData> OrderDatas = null;
	}
}
