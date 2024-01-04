using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public enum FileFormat
{
    Json,
    TSV,
    Class
}

namespace MyGoogleSheetsParser
{
    public static class DataUtility
    {
        public static string dataPath => Application.dataPath;

        public static string LoadTSV(string fileName)
        {
            string path = GetPath(fileName, FileFormat.TSV);

            if (File.Exists(path) == false)
            {
                return null;
            }

            return File.ReadAllText(path);
        }

        public static void Save(string fileName, string file, FileFormat fileFormat, string folderName = null)
        {
            WriteText(GetPath(fileName, fileFormat, folderName), file);
        }

        public static string SaveToTSVFromDoubleList(string fileName, IList<IList<object>> values)
        {
            StringBuilder sb = new StringBuilder();

            string result = string.Empty;

            for (int i = 0; i < values.Count; i++)
            {
                for (int j = 0; j < values[i].Count; j++)
                {
                    sb.Append($"{values[i][j]}\t");
                }
                sb.AppendLine();
            }

            result = sb.ToString();

            WriteText(GetPath(fileName, FileFormat.TSV), result);

            return result;
        }

        public static string SaveToTSVFromDictionary(string fileName, Dictionary<string, int> values)
        {
            StringBuilder sb = new StringBuilder();

            string result = string.Empty;

            foreach (var value in values)
            {
                sb.AppendLine($"{value.Key}\t{Math.Round((double)value.Value * 100 / 100000, 3)}%");
            }

            result = sb.ToString();

            WriteText(GetPath(fileName, FileFormat.TSV), result);

            return result;
        }

        public static string GetPath(string fileName, FileFormat fileFormat, string folderName = null)
        {
            if (fileFormat == FileFormat.Json)
            {
                return @$"{dataPath}/Resources/JsonData/{fileName}.json";
            }
            else if (fileFormat == FileFormat.TSV)
            {
                return @$"{dataPath}/Resources/TSVData/{fileName}.tsv";
            }
            else if (fileFormat == FileFormat.Class)
            {
                if (folderName == null)
                {
                    throw new ArgumentNullException(fileName + "폴더 이름이 Null임");
                }
                return @$"{dataPath}/{folderName}/Scripts/ScriptableData/Class/{fileName}.cs";
            }

            return null;
        }

        public static void WriteText(string path, string value)
        {
            if (File.Exists(path) == false)
            {
                string directory = Path.GetDirectoryName(path);
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(path, value);
            }
            else
            {
                File.WriteAllText(path, value);
            }
        }
    }
}