using JetBrains.Annotations;
using MyBattleSystem;
using MyGoogleSheetsParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ClassGenerater
{
    private static StringBuilder sb = new StringBuilder(500);

    private static List<string> classNames = new List<string>();

    private static List<string> Enums = new List<string>();

    private static int depth = 0;

    /// <summary>
    /// Class 생성기
    /// </summary>
    /// <param name="parents">부모나 인터페이스</param>
    public static void GenerateClass(string tsv, string className, string folderName, string classNamespace = null, params string[] parents)
    {
        sb.Clear();

        classNames.Add(className);

        #region namespace
        AppendLine("using System;");
        AppendLine("using UnityEngine;");
        AppendLine("using MyGoogleSheetsParser;");
        AppendLine("using System.Collections.Generic;");
        AppendLine();
        #endregion


        #region 클래스와 부모, 인터페이스
        if (classNamespace != null)
        {
            AppendLine($"namespace {classNamespace}");
            AppendLine("{");
        }

        // enum 생성
        GenerateEnum(tsv);

        AppendLine("[Serializable]");
        Append($"public class {className} ");

        if (parents != null)
        {
            Append(": ", false);

            for (int i = 0; i < parents.Length; i++)
            {
                Append($"{parents[i]}", false);

                if (i < parents.Length - 1)
                {
                    Append(", ");
                }
            }

            AppendLine();
        }
        AppendLine("{");
        #endregion

        #region 변수
        string[] rows = tsv.Split('\n');

        // 변수명 행과 자료형 행을 가져옴
        string[] variables = rows[0].Split('\t');
        string[] dataTypes = rows[1].Split('\t');

        for (int i = 0; i < variables.Length - 1; i++)
        {
            if (dataTypes[i].Equals("Enum"))
            {
                // Enum의 경우는 Enum 타입명과 변수명이 같음
                AppendLine($"public {variables[i]} {variables[i]};");
            }
            else
            {
                if (variables[i].Equals("Index"))
                {
                    AppendLine($"public {dataTypes[i]} {variables[i]} {{ get; set; }}");
                }
                else
                {
                    AppendLine($"public {dataTypes[i]} {variables[i]};");
                }
            }
        }
        AppendLine();
        #endregion

        #region 함수
        AppendLine("public void InitializeData(string[] rows)");
        AppendLine("{");

        for (int i = 0; i < variables.Length - 1; i++)
        {
            Append($"{variables[i]} = ");

            switch (dataTypes[i])
            {
                case "string":
                    AppendLine($"rows[{i}];", false);
                    break;
                case "int":
                    AppendLine($"int.Parse(rows[{i}]);", false);
                    break;
                case "float":
                    AppendLine($"float.Parse(rows[{i}]);", false);
                    break;
                case "bool":
                    AppendLine($"bool.Parse(rows[{i}]);", false);
                    break;
                case "string[]":
                    AppendLine($"rows[{i}].Split(',');", false);
                    break;
                case "int[]":
                    AppendLine($"Array.ConvertAll(rows[{i}].Split(','), int.Parse);", false);
                    break;
                case "Enum":
                    AppendLine($"({variables[i]})Enum.Parse(typeof({variables[i]}), rows[{i}]);", false);
                    break;
                case "IntData":
                    AppendLine($"new IntData(int.Parse(rows[{i}]));", false);
                    break;
                case "FloatData":
                    AppendLine($"new FloatData(float.Parse(rows[{i}]));", false);
                    break;
                case "DoubleData":
                    AppendLine($"new DoubleData(double.Parse(rows[{i}]));", false);
                    break;
                default:
                    break;
            }
        }
        AppendLine("}");
        AppendLine();

        AppendLine($"public {className} Clone()");
        AppendLine("{");
        AppendLine($"{className} cloneData = new {className}();");
        for (int i = 0; i < variables.Length - 1; i++)
        {
            Append($"cloneData.{variables[i]} = ");

            switch (dataTypes[i])
            {
                case "string[]":
                    AppendLine($"{variables[i]}.Clone();", false);
                    break;
                case "int[]":
                    AppendLine($"{variables[i]}.Clone();", false);
                    break;
                case "IntData":
                    AppendLine($"new IntData({variables[i]});", false);
                    break;
                case "FloatData":
                    AppendLine($"new FloatData({variables[i]});", false);
                    break;
                case "DoubleData":
                    AppendLine($"new DoubleData({variables[i]});", false);
                    break;
                default:
                    AppendLine($"{variables[i]};", false);
                    break;
            }
        }

        AppendLine("return cloneData;");
        AppendLine("}");

        #endregion

        AppendLine("}");
        AppendLine();

        GenerateScriptable(className);

        if (classNamespace != null)
        {
            AppendLine("}");
        }

        DataUtility.Save($"{className}Scriptable", sb.ToString(), FileFormat.Class, folderName);
    }

    private static void GenerateScriptable(string className)
    {
        AppendLine($"[CreateAssetMenu(fileName = \"{className}Scriptable\", menuName = \"MyScriptable/{className}\", order = 1)]");
        AppendLine($"public class {className}Scriptable : ScriptableObject");
        AppendLine("{");
        AppendLine($"public List<{className}> {className}s = null;");
        AppendLine($"public Dictionary<int, {className}> {className}Dic = null;");
        AppendLine("}");
    }

    private static void GenerateEnum(string tsv)
    {
        string[] rows = tsv.Split('\n');

        string[] variables = rows[0].Split('\t');
        string[] dataTypes = rows[1].Split('\t');

        List<string> enumMembers = new List<string>();

        for (int i = 0; i < variables.Length - 1; i++)
        {
            if (dataTypes[i].Equals("Enum") && Enums.Contains(variables[i]) == false)
            {
                enumMembers.Clear();

                AppendLine($"public enum {variables[i]}");
                AppendLine("{");

                for (int j = 2; j < rows.Length - 1; j++)
                {
                    string[] datas = rows[j].Split('\t');

                    enumMembers.Add(datas[i]);
                }

                // 중복제거
                foreach (var enumMember in enumMembers.Distinct())
                {
                    AppendLine($"{enumMember},");
                }

                AppendLine("}");
                AppendLine();
                Enums.Add(variables[i]);
            }
        }

    }

    public static string GeneratePartialClass(string className, string folderName, string @namespace = null)
    {
        sb.Clear();

        List<string> lowerClassNames = new List<string>();

        for (int i = 0; i < classNames.Count; i++)
        {
            // 첫번째 문자 소문자로 만들어주기위함 명명규칙맞춰주기
            lowerClassNames.Add(classNames[i].ToLowerFirstChar());
        }

        AppendLine("using System.Collections.Generic;");
        AppendLine("using UnityEngine;");
        AppendLine("using UnityEditor;");
        AppendLine("using Cysharp.Threading.Tasks;");
        AppendLine("using MyGoogleSheetsParser;");
        AppendLine();

        if (@namespace != null)
        {
            AppendLine($"namespace {@namespace}");
            AppendLine("{");
        }
        // 컴파일하기 위해 새로운 정보를 넣어줌과 언제 갱신됐는지 알 수 있음
        AppendLine($"// {DateTime.Now.TimeOfDay}");
        AppendLine($"public partial class {className}");
        AppendLine("{");

        AppendLine("[Header(\"Sheet Datas\")]");

        #region 변수
        for (int i = 0; i < classNames.Count; i++)
        {

            AppendLine("[SerializeField]");
            AppendLine($"private {classNames[i]}Scriptable {lowerClassNames[i]}Scriptable;");
            AppendLine();

            AppendLine($"public List<{classNames[i]}> {classNames[i]}s");
            AppendLine("{");
            AppendLine("get");
            AppendLine("{");
            AppendLine($"return {lowerClassNames[i]}Scriptable.{classNames[i]}s;");
            AppendLine("}");
            AppendLine("}");
            AppendLine();

            AppendLine($"private Dictionary<int, {classNames[i]}> {lowerClassNames[i]}Dic = new Dictionary<int, {classNames[i]}>();");
            AppendLine();
            /* 예시
		    [SerializeField]
		    private OrderDataScriptable orderDataScriptable;
		    
		    public List<OrderData> OrderDatas
		    {
		    	get
		    	{
		    		return orderDataScriptable.OrderDatas;
		    	}
		    }
            */
        }
        #endregion

        #region 함수

        /// <see cref="MyBattleSystem.ResourceManager.InitializeMethod"/>
        AppendLine("// 컴파일될 때 자동으로 실행됨");
        AppendLine("[InitializeOnLoadMethod]");
        AppendLine("private static void InitializeMethod()");
        AppendLine("{");
        AppendLine("// 에디터 환경일 때 ");
        AppendLine("if (Application.isPlaying == false)");
        AppendLine("{");
        AppendLine("dynamicMethod += async () => {");
        AppendLine($"await {className.ToLowerFirstChar()}.InitializeDynamicScriptable();");
        AppendLine("#if UNITY_EDITOR", false);
        AppendLine($"{className.ToLowerFirstChar()}.SaveDynamicScriptable();");
        AppendLine("#endif", false);
        AppendLine("};");
        AppendLine("}");
        AppendLine("// 플레이 중인 상태");
        AppendLine("initDataDictionarys += () => Instance.InitDataDictionarys();");
        AppendLine("}");
        AppendLine();

        // 스크립터블 초기화 함수
        /// <see cref="MyBattleSystem.ResourceManager.InitializeDynamicScriptable"/>
        AppendLine($"public async UniTask InitializeDynamicScriptable()");
        AppendLine("{");

        for (int i = 0; i < classNames.Count; i++)
        {
            AppendLine($"{lowerClassNames[i]}Scriptable = CreateScriptable<{classNames[i]}Scriptable>();");
            AppendLine($"{lowerClassNames[i]}Scriptable.{classNames[i]}s = await GoogleSheetsParser.GetSheetDatasAsync<{classNames[i]}>();");
            AppendLine();

            /* 예시
			orderDataScriptable = CreateScriptable<OrderDataScriptable>();
			orderDataScriptable.OrderDatas = await GoogleSheetsParser.GetSheetDatasAsync<OrderData>();
            */
        }
        AppendLine("}");
        AppendLine();

        /// <see cref="MyBattleSystem.ResourceManager.InitDataDictionarys"/>
        AppendLine("private void InitDataDictionarys()");
        AppendLine("{");
        for (int i = 0; i < classNames.Count; i++)
        {
            AppendLine($"{lowerClassNames[i]}Dic = AddDataDictionarty({classNames[i]}s);");
        }
        AppendLine("}");
        AppendLine();

        /// <see cref="MyBattleSystem.ResourceManager.AddDataDictionarty"/>
        AppendLine("private Dictionary<int, T> AddDataDictionarty<T>(List<T> list) where T : IGoogleSheetData");
        AppendLine("{");
        AppendLine("var dic = new Dictionary<int, T>();");
        AppendLine("");
        AppendLine("for (int i = 0; i < list.Count; i++)");
        AppendLine("{");
        AppendLine("if (dic.ContainsKey(list[i].Index) == false)");
        AppendLine("{");
        AppendLine("dic.Add(list[i].Index, list[i]);");
        AppendLine("}");
        AppendLine("}");
        AppendLine("return dic;");
        AppendLine("}");

        for (int i = 0; i < classNames.Count; i++)
        {
            AppendLine();
            AppendLine($"public void TryGet{classNames[i]}(out {classNames[i]} {lowerClassNames[i]}, int index)");
            AppendLine("{");
            AppendLine($"if ({lowerClassNames[i]}Dic.ContainsKey(index) == false)");
            AppendLine("{");
            AppendLine($"{lowerClassNames[i]} = null;");
            AppendLine("Debug.Assert(false, $\"Index가 잘못됨 : {index}\");");
            AppendLine("return;");
            AppendLine("}");
            AppendLine();
            AppendLine($"{lowerClassNames[i]} = {lowerClassNames[i]}Dic[index].Clone();");
            AppendLine("}");
        }

        AppendLine();

        // 스크립터블 저장
        /// <see cref="MyBattleSystem.ResourceManager.SaveDynamicScriptable"/>
        AppendLine("public void SaveDynamicScriptable()");
        AppendLine("{");
        for (int i = 0; i < classNames.Count; i++)
        {
            AppendLine($"EditorUtility.SetDirty({lowerClassNames[i]}Scriptable);");
        }
        AppendLine("}");
        #endregion

        AppendLine("}");

        if (@namespace != null)
        {
            Append("}");
        }

        string result = sb.ToString();

        DataUtility.Save(className, result, FileFormat.Class, folderName);
        classNames.Clear();

        return result;
    }

    private static void Append(string value, bool addDepth = true)
    {
        if (addDepth)
        {
            AddDepth(value);
        }

        sb.Append(value);

        UpdateDepth(value);
    }

    private static void AppendLine(string value = null, bool addDepth = true)
    {
        if (addDepth)
        {
            AddDepth(value);
        }

        sb.AppendLine(value);

        UpdateDepth(value);
    }

    private static void AddDepth(string value)
    {
        // 먼저 빼줘야함
        if (value != null && value.StartsWith("}") && value.EndsWith("}"))
        {
            depth--;
        }

        for (int i = 0; i < depth; i++)
        {
            sb.Append('\t');
        }
    }

    private static void UpdateDepth(string value)
    {
        if (value != null && value.StartsWith("{") && value.EndsWith("{"))
        {
            depth++;
        }
    }
}
