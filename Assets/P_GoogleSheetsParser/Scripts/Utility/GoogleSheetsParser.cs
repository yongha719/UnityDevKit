using Cysharp.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Debug = UnityEngine.Debug;

namespace MyGoogleSheetsParser
{
    public static class GoogleSheetsParser
    {
        private static string[] scopes = new string[] { SheetsService.Scope.SpreadsheetsReadonly };

        private static ClientSecrets clientSecrets = new ClientSecrets() { ClientId = "클라이언트ID", ClientSecret = "클라이언트비밀번호" };

        private static UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, "Unity Editor", CancellationToken.None).Result;

        private static SheetsService service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = applicationName
        });

        private const string applicationName = "애플리케이션이름";

        private static string[] SheetIDs = { };

        private static Dictionary<string, Spreadsheet> spreadSheetDic = new Dictionary<string, Spreadsheet>();

        // 타입에 맞게 시트 ID를 가지고 있음
        private static Dictionary<Type, string> spreadSheetIdDic = new Dictionary<Type, string>() {
            { typeof(TranslationData), TRANSLATION_SHEET_ID },
        };
        
        // 타입에 맞게 파싱된 시트의 GID를 가져옴
        private static Dictionary<Type, int> typeToGIDDic = new Dictionary<Type, int>()
        {
            { typeof(TranslationData), 0 },
        };

        /// <summary>
        /// 자동 불러오기에 추가된 시트의 형식에 맞게 Class 생성
        /// </summary>
        public static async UniTask LoadSheetsAndCreateClass(string classNamespace, string folderName)
        {
            for (int i = 0; i < SheetIDs.Length; i++)
            {
                Spreadsheet spreadsheet = await service.Spreadsheets.Get(SheetIDs[i]).ExecuteAsync();

                for (int sheetIndex = 0; sheetIndex < spreadsheet.Sheets.Count; sheetIndex++)
                {
                    Sheet sheet = spreadsheet.Sheets[sheetIndex];

                    string sheetName = sheet.Properties.Title;

                    // 안쓰는 시트라는 뜻
                    if (sheetName.StartsWith("*"))
                    {
                        Debug.Assert(false, $"사용하지 않는 시트가 있습니다. 확인해주세요\n시트 이름 : {sheetName}");
                        continue;
                    }

                    ResourceManager.StartLoadData(sheetName);

                    // Api에서 자동으로 데이터만 걸러줌
                    ValueRange request = await service.Spreadsheets.Values.Get(SheetIDs[i], sheetName + "!A1:Z").ExecuteAsync();
                    IList<IList<object>> values = request.Values;

                    ProcessingData(values);
                    string tsv = DataUtility.SaveToTSVFromDoubleList(sheetName, values);

                    ClassGenerater.GenerateClass(tsv, sheet.Properties.Title, folderName, classNamespace, parents: nameof(IGoogleSheetData));

                    ResourceManager.FinishLoadData(sheetName);
                }
            }
        }

        /// <summary>
        /// TSV 파일이 있다면 TSV 파일 파싱<br></br>
        /// 파일이 없다면 시트 다운로드
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>파싱된 데이터의 List로 반환</returns>
        public static async UniTask<List<T>> GetSheetDatasAsync<T>() where T : class, IGoogleSheetData, new()
        {
            string tsv = DataUtility.LoadTSV(typeof(T).Name);

            if (tsv == null)
            {
                tsv = await LoadData<T>();
            }

            List<T> sheetDatas = ParseSheetData<T>(tsv.Split('\n'));

            return sheetDatas;
        }

        /// <summary>
        /// Sheet에서 Data받아와서 TSV로 파싱
        /// </summary>
        /// <typeparam name="T">받고 싶은 데이터 형식</typeparam>
        /// <returns>TSV 형식의 string</returns>
        public static async UniTask<string> LoadData<T>()
        {
            Type dataType = typeof(T);

            ResourceManager.StartLoadData(dataType.Name);

            string spreadSheetID = spreadSheetIdDic[dataType];

            Spreadsheet spreadsheet = null;

            if (spreadSheetDic.ContainsKey(spreadSheetID) == false)
            {
                spreadsheet = await service.Spreadsheets.Get(spreadSheetID).ExecuteAsync();

                // await으로 기다리는 동안에 key가 추가되었을 경우 예외 처리
                if (spreadSheetDic.ContainsKey(spreadSheetID) == false)
                {
                    spreadSheetDic.Add(spreadSheetID, spreadsheet);
                }
                else
                {
                    spreadSheetDic[spreadSheetID] = spreadsheet;
                }
            }
            else
            {
                spreadsheet = spreadSheetDic[spreadSheetID];
            }

            Sheet sheet = GetSheetByType<T>(spreadsheet);

            string sheetName = sheet.Properties.Title;

            // 안쓰는 시트라는 뜻
            if (sheetName.StartsWith("*"))
            {
                throw new Exception($"사용하지 않는 시트가 있습니다. 확인해주세요\n시트 이름 : {sheetName}");
            }

            // Api에서 자동으로 데이터만 걸러줌
            ValueRange request = await service.Spreadsheets.Values.Get(spreadSheetID, sheetName + "!A1:Z").ExecuteAsync();
            IList<IList<object>> values = request.Values;

            ProcessingData(values);

            ResourceManager.FinishLoadData(dataType.Name);

            string tsv = DataUtility.SaveToTSVFromDoubleList(dataType.Name, values);

            return tsv;
        }

        private static Sheet GetSheetByType<T>(Spreadsheet spreadsheet)
        {
            if (TryGetGIDByType(out int gid, typeof(T)))
            {
                for (int i = 0; i < spreadsheet.Sheets.Count; i++)
                {
                    if (spreadsheet.Sheets[i].Properties.SheetId == gid)
                    {
                        return spreadsheet.Sheets[i];
                    }
                }
            }

            return null;
        }

        private static bool TryGetGIDByType(out int gid, Type type)
        {
            if (typeToGIDDic.ContainsKey(type) == false)
            {
                Debug.Assert(false, $"Type이 잘못됨 {type}");
                gid = int.MinValue;
                return false;
            }

            gid = typeToGIDDic[type];
            return true;
        }

        /// <summary>
        /// 필요없는 데이터를 제거하는 작업
        /// </summary>
        private static void ProcessingData(IList<IList<object>> sheetDatas)
        {
            List<int> removeColumnIndex = new List<int>();

            // 0번째에 컬럼명들이 들어가있음
            List<string> columnNames = sheetDatas[0].Cast<string>().ToList();

            for (int i = 0; i < columnNames.Count; i++)
            {
                // *으로 시작하면 주석이나 안쓰는 데이터임
                if (columnNames[i].StartsWith("*"))
                {
                    removeColumnIndex.Add(i);
                }
            }

            for (int i = removeColumnIndex.Count - 1; i >= 0; i--)
            {
                foreach (var value in sheetDatas)
                {
                    value.RemoveAt(removeColumnIndex[i]);
                }
            }

            // 실제 필요한 컬럼수
            int columnCount = columnNames.Count - removeColumnIndex.Count;

            // 실제 필요한 컬럼 수만큼만 데이터 가져옴 (옆에 있는 주석같은거 걸러주는 작업)
            for (int i = 1; i < sheetDatas.Count; i++)
            {
                if (sheetDatas[i].Count > columnCount)
                {
                    var overCount = sheetDatas[i].Count - columnCount;

                    // removeRange랑 같습니다 컬럼보다 많으면 지워주는 역할입니다
                    for (int deleteIndex = columnCount + overCount - 1; deleteIndex >= columnCount; deleteIndex--)
                    {
                        sheetDatas[i].RemoveAt(deleteIndex);
                    }
                }
            }
        }

        /// <summary>
        /// TSV 파일 파싱해서 List로 반환해줌
        /// </summary>
        /// <param name="rows">TSV 파일의 행을 인자로 받음</param>
        private static List<T> ParseSheetData<T>(string[] rows) where T : class, IGoogleSheetData, new()
        {
            List<T> sheetDatas = new List<T>();

            // 3번째부터 데이터임 마지막은 빈칸이어서 걸러줌
            for (int i = 2; i < rows.Length - 1; i++)
            {
                string[] datas = rows[i].Split('\t');

                // 받은 타입에 따라 데이터를 초기화해줌
                T sheetData = new T();
                sheetData.InitializeData(datas);

                sheetDatas.Add(sheetData);
            }

            return sheetDatas;
        }

    }
}