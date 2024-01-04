using MyGoogleSheetsParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = System.Random;


namespace MyGoogleSheetsParser
{

    [Serializable]
    public class RandomPicker
    {
        private Dictionary<string, double> candidatesDic = new Dictionary<string, double>();

        private Random random = new Random();

        private int totalWeight;

        public int TotalWeight
        {
            get
            {
                return totalWeight;
            }
        }

        /// <summary>
        /// 데이터 초기화
        /// </summary>
        public void InitializeData(List<RandomPickData> pickDatas)
        {
            totalWeight = 0;

            candidatesDic.Clear();

            if (pickDatas.Count() == 0)
            {
                Debug.Assert(false, "초기화할 데이터가 없음");
            }

            Add(pickDatas);
        }

        /// <summary>
        /// list 형태로 <see cref="RandomPickData"/>를 추가하고 싶을 때
        /// </summary>
        public void Add(List<RandomPickData> pickDatas)
        {
            foreach (RandomPickData pickData in pickDatas)
            {
                if (candidatesDic.ContainsKey(pickData.Title) == false)
                {
                    candidatesDic.Add(pickData.Title, pickData.Weight);

                    totalWeight += pickData.Weight;
                }
            }
        }

        /// <summary>
        /// 가중치 계산해서 랜덤으로 Title을 가져옴
        /// </summary>
        public string GetRandomTitle()
        {
            int randomValue = random.Next(totalWeight);

            double current = 0;

            foreach (var candidate in candidatesDic)
            {
                current += candidate.Value;

                if (randomValue < current)
                {
                    return candidate.Key;
                }
            }

            return null;
        }
    }
}
