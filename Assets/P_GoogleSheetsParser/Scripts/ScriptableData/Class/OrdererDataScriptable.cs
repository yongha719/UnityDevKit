using System;
using UnityEngine;
using MyGoogleSheetsParser;
using System.Collections.Generic;

namespace MyGoogleSheetsParser
{
	[Serializable]
	public class OrdererData : IGoogleSheetData	
	{
		public int Index;
		public string Name;
		public string[] Orderers;

        int IGoogleSheetData.Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void InitializeData(string[] rows)
		{
			Index = int.Parse(rows[0]);
			Name = rows[1];
			Orderers = rows[2].Split(',');
		}
	}
	
	[CreateAssetMenu(fileName = "OrdererDataScriptable", menuName = "MyScriptable/OrdererData", order = 1)]
	public class OrdererDataScriptable : ScriptableObject
	{
		public List<OrdererData> OrdererDatas = null;
	}
}
