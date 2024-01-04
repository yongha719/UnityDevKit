using System;
using UnityEngine;
using MyGoogleSheetsParser;
using System.Collections.Generic;

namespace MyGoogleSheetsParser
{
	[Serializable]
	public class TestingSheet : IGoogleSheetData	
	{
		public int index;
		public int myint;
		public string mystring;

        public int Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void InitializeData(string[] rows)
		{
			index = int.Parse(rows[0]);
			myint = int.Parse(rows[1]);
			mystring = rows[2];
		}
	}
	
	[CreateAssetMenu(fileName = "TestingSheetScriptable", menuName = "MyScriptable/TestingSheet", order = 1)]
	public class TestingSheetScriptable : ScriptableObject
	{
		public List<TestingSheet> TestingSheets = null;
	}
}
