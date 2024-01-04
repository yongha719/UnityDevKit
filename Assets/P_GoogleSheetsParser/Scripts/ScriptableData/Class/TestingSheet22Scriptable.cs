using System;
using UnityEngine;
using MyGoogleSheetsParser;
using System.Collections.Generic;

namespace MyGoogleSheetsParser
{
	[Serializable]
	public class TestingSheet22 : IGoogleSheetData	
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
	
	[CreateAssetMenu(fileName = "TestingSheet22Scriptable", menuName = "MyScriptable/TestingSheet22", order = 1)]
	public class TestingSheet22Scriptable : ScriptableObject
	{
		public List<TestingSheet22> TestingSheet22s = null;
	}
}
