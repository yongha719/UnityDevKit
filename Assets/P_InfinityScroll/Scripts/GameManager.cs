using System.Collections.Generic;
using UnityEngine;

namespace MyInfinityScrollUI
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField]
        private int dataSize = 0;

        private List<string> textDatas;

        [SerializeField]
        private IS_NameScroll IS_NameScroll;

        [SerializeField]
        private List<NameItemData> NameItemDatas = new List<NameItemData>();

        public int DataCount
        {
            get
            {
                return dataSize;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            textDatas = new List<string>(dataSize);

            for (int i = 0; i < dataSize; i++)
            {
                textDatas.Add(i.ToString());
            }
        }

        private void Start()
        {
            IS_NameScroll.InitializeData(NameItemDatas);
        }

        public string GetIndexString(int index)
        {
            if (index >= 0 && index < dataSize)
            {
                return textDatas[index];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}