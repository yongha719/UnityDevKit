using System;
using UnityEngine;
using MyGoogleSheetsParser;
using System.Collections.Generic;

public enum UnitsType
{
	Otherworld,
	Elf,
	Orc,
	Undead,
}

public enum UnitsCareerType
{
	Archer,
	Warrior,
	Wizard,
}

namespace MyGoogleSheetsParser
{
	[Serializable]
	public class UnitsData : IGoogleSheetData	
	{
		public int Index;
		public string Name;
		public int Count;
		public int Level;
		public UnitsType UnitsType;
		public UnitsCareerType UnitsCareerType;

        int IGoogleSheetData.Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void InitializeData(string[] rows)
		{
			Index = int.Parse(rows[0]);
			Name = rows[1];
			Count = int.Parse(rows[2]);
			Level = int.Parse(rows[3]);
			UnitsType = (UnitsType)Enum.Parse(typeof(UnitsType), rows[4]);
			UnitsCareerType = (UnitsCareerType)Enum.Parse(typeof(UnitsCareerType), rows[5]);
		}
	}
	
	[CreateAssetMenu(fileName = "UnitsDataScriptable", menuName = "MyScriptable/UnitsData", order = 1)]
	public class UnitsDataScriptable : ScriptableObject
	{
		public List<UnitsData> UnitsDatas = null;
	}
}
