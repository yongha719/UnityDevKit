using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cysharp.Threading.Tasks;

namespace MyGoogleSheetsParser
{
	// 14:03:48.3666971
	public partial class ResourceManager
	{
		[Header("Sheet Datas")]
		[SerializeField]
		private OrdererDataScriptable ordererDataScriptable;
		
		public List<OrdererData> OrdererDatas
		{
			get
			{
				return ordererDataScriptable.OrdererDatas;
			}
		}
		
		[SerializeField]
		private OrderDataScriptable orderDataScriptable;
		
		public List<OrderData> OrderDatas
		{
			get
			{
				return orderDataScriptable.OrderDatas;
			}
		}
		
		[SerializeField]
		private UnitsDataScriptable unitsDataScriptable;
		
		public List<UnitsData> UnitsDatas
		{
			get
			{
				return unitsDataScriptable.UnitsDatas;
			}
		}
		
		[SerializeField]
		private SpiritDataScriptable spiritDataScriptable;
		
		public List<SpiritData> SpiritDatas
		{
			get
			{
				return spiritDataScriptable.SpiritDatas;
			}
		}
		
		[SerializeField]
		private SpiritAttributeIndexDataScriptable spiritAttributeIndexDataScriptable;
		
		public List<SpiritAttributeIndexData> SpiritAttributeIndexDatas
		{
			get
			{
				return spiritAttributeIndexDataScriptable.SpiritAttributeIndexDatas;
			}
		}
		
		[SerializeField]
		private RandomPickDataScriptable randomPickDataScriptable;
		
		public List<RandomPickData> RandomPickDatas
		{
			get
			{
				return randomPickDataScriptable.RandomPickDatas;
			}
		}
		
		[SerializeField]
		private TestingSheetScriptable testingSheetScriptable;
		
		public List<TestingSheet> TestingSheets
		{
			get
			{
				return testingSheetScriptable.TestingSheets;
			}
		}
		
		[SerializeField]
		private TestingSheet22Scriptable testingSheet22Scriptable;
		
		public List<TestingSheet22> TestingSheet22s
		{
			get
			{
				return testingSheet22Scriptable.TestingSheet22s;
			}
		}
		
		// 컴파일될 때 자동으로 실행됨
		[InitializeOnLoadMethod]
		private static void InitializeMethod()
		{
			dynamicMethod += () => {
			resourceManager.InitializeDynamicScriptable().Forget();
			#if UNITY_EDITOR
			resourceManager.SaveDynamicScriptable();
			#endif
			};
		}
		
		public async UniTask InitializeDynamicScriptable()
		{
			ordererDataScriptable = CreateScriptable<OrdererDataScriptable>();
			ordererDataScriptable.OrdererDatas = await GoogleSheetsParser.GetSheetDatasAsync<OrdererData>();
			
			orderDataScriptable = CreateScriptable<OrderDataScriptable>();
			orderDataScriptable.OrderDatas = await GoogleSheetsParser.GetSheetDatasAsync<OrderData>();
			
			unitsDataScriptable = CreateScriptable<UnitsDataScriptable>();
			unitsDataScriptable.UnitsDatas = await GoogleSheetsParser.GetSheetDatasAsync<UnitsData>();
			
			spiritDataScriptable = CreateScriptable<SpiritDataScriptable>();
			spiritDataScriptable.SpiritDatas = await GoogleSheetsParser.GetSheetDatasAsync<SpiritData>();
			
			spiritAttributeIndexDataScriptable = CreateScriptable<SpiritAttributeIndexDataScriptable>();
			spiritAttributeIndexDataScriptable.SpiritAttributeIndexDatas = await GoogleSheetsParser.GetSheetDatasAsync<SpiritAttributeIndexData>();
			
			randomPickDataScriptable = CreateScriptable<RandomPickDataScriptable>();
			randomPickDataScriptable.RandomPickDatas = await GoogleSheetsParser.GetSheetDatasAsync<RandomPickData>();
			
			testingSheetScriptable = CreateScriptable<TestingSheetScriptable>();
			testingSheetScriptable.TestingSheets = await GoogleSheetsParser.GetSheetDatasAsync<TestingSheet>();
			
			testingSheet22Scriptable = CreateScriptable<TestingSheet22Scriptable>();
			testingSheet22Scriptable.TestingSheet22s = await GoogleSheetsParser.GetSheetDatasAsync<TestingSheet22>();
			
		}
		
		public void SaveDynamicScriptable()
		{
			EditorUtility.SetDirty(ordererDataScriptable);
			EditorUtility.SetDirty(orderDataScriptable);
			EditorUtility.SetDirty(unitsDataScriptable);
			EditorUtility.SetDirty(spiritDataScriptable);
			EditorUtility.SetDirty(spiritAttributeIndexDataScriptable);
			EditorUtility.SetDirty(randomPickDataScriptable);
			EditorUtility.SetDirty(testingSheetScriptable);
			EditorUtility.SetDirty(testingSheet22Scriptable);
		}
	}
}