using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Directory = System.IO.Directory;

namespace MyGoogleSheetsParser
{
    public partial class ResourceManager : Singleton<ResourceManager>
    {
        private Dictionary<int, List<ProbabilityData>> groupProbabilitys = new Dictionary<int, List<ProbabilityData>>();

        [Tooltip("번역 데이터")]
        public TranslationDataScriptable TranslationDataScriptable;

        public List<TranslationData> TranslationDatas
        {
            get
            {
                return TranslationDataScriptable.Translations;
            }
        }

        private Dictionary<string, List<string>> translatedLanguagesDic = new Dictionary<string, List<string>>();

        [Tooltip("로딩중인 데이터 리스트")]
        public static SortedSet<string> loadingDataList = new SortedSet<string>();

        private static Stopwatch stopWatch = new Stopwatch();

        [Tooltip("Partial 클래스에서 이벤트로 등록할 초기화될 때 함수")]
        private static Action dynamicMethod = null;

        private static ResourceManager resourceManager;

        private static bool isDownloading = false;

        protected override void Awake()
        {
            base.Awake();

            InitializeTranslationData();
        }
        xdxds
        #region Scriptable Load and Save

        // 시트 자동 불러오기 및 컴파일
        // 자동 불러오기 추가하고 싶다면 GoogleSheetsParser.SheetIDs 에 시트 ID 추가해야 함
        [Button("Download Sheet Datas", enabledMode: EButtonEnableMode.Editor), Conditional("UNITY_EDITOR")]
        public async void LoadSheetDatas()
        {
            print("데이터 로드 시작");
            isDownloading = true;

            stopWatch.Start();
            loadingDataList.Clear();

            await GoogleSheetsParser.LoadSheetsAndCreateClass(nameof(MyGoogleSheetsParser), "P_GoogleSheetsParser");
            await GoogleSheetsParser.LoadData<TranslationData>();
            print("컴파일을 기다려주세요");

            ClassGenerater.GeneratePartialClass(nameof(ResourceManager), nameof(MyGoogleSheetsParser));
            // 컴파일
            AssetDatabase.Refresh();
        }

        // 시트 불러온후 컴파일이 끝난뒤 실행해야함
        [Button(enabledMode: EButtonEnableMode.Editor), Conditional("UNITY_EDITOR")]
        public void InitializeAllScriptableObjects()
        {
            if (isDownloading)
            {
                Debug.Log("다운로드 중입니다. 다운로드를 기다려주세요.");
                return;
            }

            string directory = "Assets/Resources/Scriptable";
            if (Directory.Exists(directory) == false)
            {
                Debug.Log("경로 생성");
                Directory.CreateDirectory(directory);
            }

            // 스크립터블 오브젝트 데이터 초기화
            InitializeScriptable().Forget();
            // 스크립터블 오브젝트 저장
            SaveScriptable();

            resourceManager = this;

            if (dynamicMethod == null)
            {
                Debug.Assert(false, "dynamicMethod가 Null임 Partial 클래스 확인바람");
            }
            else
            {
                dynamicMethod();
                dynamicMethod = null;

                Debug.Log("Scriptable Objects 데이터 초기화 완료");
            }
        }

        [Button("Remove All Scriptable", enabledMode: EButtonEnableMode.Editor), Conditional("UNITY_EDITOR")]
        public void RemoveClassAndScriptable()
        {
            if (isDownloading)
            {
                Debug.Log("다운로드 중입니다. 다운로드를 기다려주세요.");
                return;
            }

            AssetDatabase.DeleteAsset("Assets/Resources/Scriptable/");
            AssetDatabase.Refresh();

            Debug.Log("Scriptable Objects 삭제 완료");
        }
        #endregion

        /// <summary>
        /// 자동 불러오기가 아닌 데이터들
        /// </summary>
        private async UniTask InitializeScriptable()
        {
            TranslationDataScriptable = CreateScriptable<TranslationDataScriptable>();
            TranslationDataScriptable.Translations = await GoogleSheetsParser.GetSheetDatasAsync<TranslationData>();
        }

        /// <summary>
        /// 자동 불러오기가 아닌 데이터들
        /// </summary>
        private void SaveScriptable()
        {
            EditorUtility.SetDirty(TranslationDataScriptable);
        }

        private T CreateScriptable<T>() where T : ScriptableObject
        {
            T scriptable = null;

            string path = $"Assets/Resources/Scriptable/{typeof(T).Name}.asset";
            if (File.Exists(path))
            {
                scriptable = Resources.Load<T>($"Scriptable/{typeof(T).Name}");
            }
            else
            {
                scriptable = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(scriptable, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return scriptable;
        }

        // Unity에서만 실행하도록
        [Conditional("UNITY_EDITOR")]
        public static void StartLoadData(string name)
        {
            loadingDataList.Add(name);

            if (stopWatch.IsRunning == false)
            {
                stopWatch.Restart();
            }
        }

        // Unity에서만 실행하도록
        [Conditional("UNITY_EDITOR")]
        public static void FinishLoadData(string name)
        {
            StringBuilder sb = new StringBuilder();

            loadingDataList.Remove(name);
            sb.AppendLine($"{name} 다운로드 완료");

            if (loadingDataList.Count != 0)
            {
                sb.AppendLine("=========================================================");
                sb.AppendLine("남은 데이터들");
                foreach (var loadingData in loadingDataList)
                {
                    sb.AppendLine($"{loadingData} 남았음");
                }
                sb.AppendLine("=========================================================");
            }
            else
            {
                stopWatch.Stop();
                sb.AppendLine($"총 {stopWatch.Elapsed} 걸림");
            }

            print(sb.ToString());
        }

        #region Translation Methods
        /// <summary>
        /// translatedLanguagesDic 초기화 Index와 List
        /// </summary>
        public void InitializeTranslationData()
        {
            for (int i = 0; i < TranslationDatas.Count; i++)
            {
                translatedLanguagesDic.Add(TranslationDatas[i].Index, TranslationDatas[i].translatedLanguageList);
            }
        }

        /// <summary>
        /// 번역 시트에 있는 Index로 현재 언어에 맞는 string을 가져옴
        /// </summary>
        public string GetTranslatedLanguage(string index)
        {
            if (translatedLanguagesDic.ContainsKey(index))
            {
                List<string> translatedLanguage = translatedLanguagesDic[index];

                return translatedLanguage[(int)GameManager.Instance.TranslationType];
            }

            Debug.Assert(false, $"index가 잘못됨 {{{index}}}");
            // 잘못된 데이터라고 알려주기 위함
            return $"ERROR_{{{index}}}";
        }
        #endregion

        #region RandomPick Methods
        /// <summary>
        /// - Group Index로 <see cref="RandomPickData"/>를 반환
        /// </summary>
        public List<RandomPickData> GetGroupByIndex(int groupIndex)
        {
            List<RandomPickData> randomPickDatas = new List<RandomPickData>();

            foreach (var data in RandomPickDatas)
            {
                if (data.GroupID == groupIndex)
                {
                    randomPickDatas.Add(data);
                }
            }

            return randomPickDatas;
        }

        /// <summary>
        /// - Group Index로 Group의 총 가중치를 구함
        /// </summary>
        /// <param name="groupIndex"></param>
        /// <returns></returns>
        public int TotalWeightOfGroup(int groupIndex)
        {
            return GetGroupByIndex(groupIndex).Sum(x => x.Weight);
        }

        /// <summary>
        /// - Group Index로 <see cref="RandomPickData"/>를 가져와서
        /// <see cref="ProbabilityData"/> List를 만들어 반환
        /// <br></br><br></br>
        /// 해당 Group의 확률표를 만들기 위한 데이터들
        /// </summary>
        public List<ProbabilityData> GetGroupProbabilitys(int groupIndex)
        {
            if (groupProbabilitys.ContainsKey(groupIndex) == false)
            {
                // Infinity Scroll Data
                List<ProbabilityData> result = new List<ProbabilityData>();

                int totalWeight = TotalWeightOfGroup(groupIndex);

                foreach (var data in GetGroupByIndex(groupIndex))
                {
                    ProbabilityData probabilityData = new ProbabilityData();

                    // 타이틀과 확률
                    probabilityData.Init(data.Title,
                        probability: Math.Round((double)data.Weight * 100 / totalWeight, 3));
                    result.Add(probabilityData);
                }

                groupProbabilitys.Add(groupIndex, result);
            }

            return groupProbabilitys[groupIndex];
        }

        /// <summary>
        /// Title로 Index를 가져오기 위해 만들었음
        /// </summary>
        public int GetIndexByTitle(string title)
        {
            for (int i = 0; i < RandomPickDatas.Count; i++)
            {
                if (RandomPickDatas[i].Title == title)
                    return RandomPickDatas[i].Index;
            }

            Debug.Assert(false, $"없는 title임 {title}");
            return int.MinValue;
        }

        #endregion
    }
}
