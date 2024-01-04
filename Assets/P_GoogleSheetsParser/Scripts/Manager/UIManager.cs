using MyInfinityScrollUI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MyGoogleSheetsParser
{
    public class UIManager : Singleton<UIManager>
    {
        private RandomPicker randomPicker = new RandomPicker();

        private int groupIndex = 0;

        // 확률 검증용 Key : Title, Value : Count
        private Dictionary<string, int> ProbabilityVerificationDic = new Dictionary<string, int>(10);

        [SerializeField]
        private TranslationText titleText;

        [SerializeField]
        private TextMeshProUGUI drawNameText;

        [SerializeField]
        private InfinityScroll probabilityScroll;

        [SerializeField]
        private ParticleSystem drawParticle;

        private Dictionary<int, string> indexToTitleDic = new Dictionary<int, string>();

        protected override void Awake()
        {
            base.Awake();

            // 뽑기 종류에 따른 번역 Index
            indexToTitleDic.Add(1, "Draw_Unit");
            indexToTitleDic.Add(2, "Draw_Spirit");
            indexToTitleDic.Add(3, "Draw_Tier");
        }

        private void Start()
        {
            drawParticle = Instantiate(drawParticle);

            // 병사 뽑기로 셋팅
            SetRandomGroup(1);
        }

        /// <summary>
        /// 변경된 언어로 업데이트
        /// </summary>
        public void OnEnable()
        {
            // 뽑기 종류
            drawNameText.text = ResourceManager.Instance.GetTranslatedLanguage(indexToTitleDic[1]);

            var probabilitys = ResourceManager.Instance.GetGroupProbabilitys(groupIndex);
            probabilityScroll.InitializeData(probabilitys);
        }

        // Inspector에서 참조
        // Buttons/Draws
        /// <summary>
        /// Group Index에 맞게 띄워줄 UI 셋팅
        /// </summary>
        public void SetRandomGroup(int _groupIndex)
        {
            if (groupIndex != _groupIndex)
            {
                titleText.SetOriginText();

                // 데이터 갱신
                drawNameText.text = ResourceManager.Instance.GetTranslatedLanguage(indexToTitleDic[_groupIndex]);

                var pickDatas = ResourceManager.Instance.GetGroupByIndex(_groupIndex);
                randomPicker.InitializeData(pickDatas);

                var probabilitys = ResourceManager.Instance.GetGroupProbabilitys(_groupIndex);
                probabilityScroll.InitializeData(probabilitys);
            }

            groupIndex = _groupIndex;
        }

        // Inspector에서 참조
        // Draw Button
        /// <summary>
        /// 현재 셋팅되어 있는 RandomData에서 하나 가져옴 
        /// </summary>
        public void GetRandomTitle()
        {
            string title = ResourceManager.Instance.GetTranslatedLanguage(randomPicker.GetRandomTitle());
            titleText.text = title;

            // 파티클 실행
            if (drawParticle.isPlaying)
            {
                drawParticle.Clear();
            }
            drawParticle.Play();
        }

        // Inspector에서 참조
        // Probability Verification Button
        /// <summary>
        /// 확률 검증하는 함수
        /// </summary>
        public void ProbabilityVerificationDraw()
        {
            ProbabilityVerificationDic.Clear();

            for (int i = 0; i < 100000; i++)
            {
                var title = randomPicker.GetRandomTitle();

                if (ProbabilityVerificationDic.ContainsKey(title))
                {
                    ProbabilityVerificationDic[title]++;
                }
                else
                {
                    ProbabilityVerificationDic.Add(title, 1);
                }
            }

            DataUtility.SaveToTSVFromDictionary($"GroupID {groupIndex}", ProbabilityVerificationDic);
        }
    }
}
