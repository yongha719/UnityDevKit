using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyInfinityScrollUI
{
    [AddComponentMenu("My/InfinityScroll", 0)]
    public class InfinityScroll : ScrollRect
    {
        protected RectTransform scrollRectTransform;

        protected List<ScrollItemBase> scrollItemList = new List<ScrollItemBase>();

        protected List<ScrollItemDataBase> scrollItemDataList = new List<ScrollItemDataBase>();

        protected Queue<ScrollItemBase> scrollItemQueue = new Queue<ScrollItemBase>();

        [SerializeField]
        protected int indexMin;
        [SerializeField]
        protected int indexMax;

        [SerializeField, Tooltip("Scroll Item이 보이는 위치 Max")]
        protected float scrollItemVisibleExtendMaxValue;
        [SerializeField, Tooltip("Scroll Item이 보이는 위치 Min")]
        protected float scrollItemVisibleExtendMinValue;

        protected float viewportWidth;
        protected float viewportHeight;

        [SerializeField]
        Image image_InactiveMaxPos;
        [SerializeField]
        Image image_InactiveMinPos;

        [SerializeField]
        protected float spacing;

        // Contents 사이즈
        public Vector2 scrollItemSize;

        [SerializeField, Tooltip("ScrollItem Prefab")]
        protected ScrollItemBase scrollItemPrefab;
        private int dataCount;

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying == false)
                return;

            scrollRectTransform = (RectTransform)transform;

            viewportWidth = scrollRectTransform.rect.width;
            viewportHeight = scrollRectTransform.rect.height;

            scrollItemSize = scrollItemPrefab.Size;
            CreateItems();
        }

        protected override void Start()
        {
            base.Start();

            onValueChanged.AddListener(OnValueChanged);

            if (image_InactiveMaxPos != null && image_InactiveMinPos != null)
            {
                if (vertical)
                {
                    // 범위 표시용 Image
                    image_InactiveMaxPos.transform.localRotation = Quaternion.identity;
                    image_InactiveMinPos.transform.localRotation = Quaternion.identity;

                    image_InactiveMaxPos.rectTransform.anchoredPosition = new Vector2(0, viewportHeight / 2 + scrollItemVisibleExtendMaxValue);
                    image_InactiveMinPos.rectTransform.anchoredPosition = new Vector2(0, -(viewportHeight / 2 + scrollItemVisibleExtendMinValue));
                }
                else if (horizontal)
                {
                    image_InactiveMaxPos.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    image_InactiveMinPos.transform.localRotation = Quaternion.Euler(0, 0, 90);

                    image_InactiveMaxPos.rectTransform.anchoredPosition = new Vector2(-(viewportWidth / 2 + scrollItemVisibleExtendMaxValue), 0);
                    image_InactiveMinPos.rectTransform.anchoredPosition = new Vector2(viewportWidth / 2 + scrollItemVisibleExtendMinValue, 0);
                }
            }
        }

        /// <summary>
        /// vertical, horizotal일 때 viewport의 크기에 맞춰 일정 갯수만큼 생성
        /// </summary>
        protected void CreateItems()
        {
            int createCount = 0;

            if (vertical)
            {
                createCount = (int)((viewportHeight + scrollItemVisibleExtendMaxValue + scrollItemVisibleExtendMinValue) / (scrollItemSize.y + spacing));
            }
            else if (horizontal)
            {
                createCount = (int)((viewportWidth + scrollItemVisibleExtendMaxValue + scrollItemVisibleExtendMinValue) / (scrollItemSize.x + spacing));
            }

            // Viewport안에 들어갈만큼 생성한뒤 여유분으로 2개 더 추가
            createCount += 2;

            for (int i = 0; i < createCount; i++)
            {
                var itemBase = Instantiate(scrollItemPrefab, content);
                scrollItemQueue.Enqueue(itemBase);
                itemBase.SetActive(false);
            }
        }

        /// <summary>
        /// DataList를 받아서 초기화 해주는 역할
        /// </summary>
        public void InitializeData<T>(List<T> dataList) where T : ScrollItemDataBase
        {
            // 다른 데이터들 있을 때 한번 초기화 해줌
            Clear();

            for (int i = 0; i < dataList.Count; i++)
            {
                scrollItemList.Add(null);
            }
            scrollItemDataList.AddRange(dataList);

            OnValueChanged(normalizedPosition);

            dataCount = dataList.Count;

            SetContentSize();
        }

        protected void SetContentSize()
        {
            float size = 0f;

            for (int i = 0; i < scrollItemDataList.Count; i++)
            {
                size += vertical ? scrollItemSize.y : scrollItemSize.x;
                size += spacing;
            }

            size -= spacing;

            if (vertical)
            {
                content.sizeDelta = new Vector2(viewportWidth, size);
            }
            else if (horizontal)
            {
                content.sizeDelta = new Vector2(size, viewportHeight);
            }
        }

        /// <summary> <see cref="onValueChanged"/> 이벤트에 등록할 함수입니다 </summary>
        protected void OnValueChanged(Vector2 pos)
        {
            if (vertical)
            {
                OnValueChanged_Scroll(content.anchoredPosition.y, viewportHeight, scrollItemSize.y, _GetScrollItemPos: (_contentSize) =>
                {
                    return new Vector2(viewportWidth / 2 - scrollItemSize.x / 2, -_contentSize);
                });
            }
            else if (horizontal)
            {
                OnValueChanged_Scroll(-content.anchoredPosition.x, viewportWidth, scrollItemSize.x, _GetScrollItemPos: (_contentSize) =>
                {
                    return new Vector2(_contentSize, -viewportHeight / 2 + scrollItemSize.y / 2);
                });
            }
        }

        /// <summary>
        /// Horizontal, Vertical Scroll 통합<br></br>
        /// 스크롤 될 때 새로 고침해야 할 것들
        /// </summary>
        /// <param name="_contentPos"> x or y of content anchoredPosition</param>
        /// <param name="_viewportInterval"> width or height of viewport</param>
        /// <param name="_scrollItemInterval"> size x or y of contentItem</param>
        /// <param name="_GetScrollItemPos"> Scroll 방식에 맞는 ContentItem의 포지션 설정 방식</param>
        protected void OnValueChanged_Scroll(float _contentPos, float _viewportInterval, float _scrollItemInterval, Func<float, Vector2> _GetScrollItemPos)
        {
            // 보이는 범위
            // x = left, y = right
            Vector2 viewportRange = new Vector2(_contentPos - scrollItemVisibleExtendMaxValue, _contentPos + _viewportInterval + scrollItemVisibleExtendMinValue);

            float _contentSize = 0f;

            // 안쓰는 Item 확인
            for (int i = 0; i < scrollItemDataList.Count; i++)
            {
                // x = left, y = right
                var scrollItemRange = new Vector2(_contentSize, _contentSize + _scrollItemInterval);

                if (scrollItemRange.y < viewportRange.x || scrollItemRange.x > viewportRange.y)
                {
                    ReleaseScrollItem(i);
                }

                _contentSize += _scrollItemInterval + spacing;
            }

            _contentSize = 0f;

            // Item써야하는지 확인
            for (int i = 0; i < scrollItemDataList.Count; i++)
            {
                // x = left, y = right
                var scrollItemRange = new Vector2(_contentSize, _contentSize + _scrollItemInterval);

                // ScrollItem이 범위안에 들어왔을 때
                if (scrollItemRange.y >= viewportRange.x && scrollItemRange.x <= viewportRange.y)
                {
                    GetScrollItem(i, _GetScrollItemPos(_contentSize));
                    scrollItemList[i].rectTransform.SetAsLastSibling();
                }

                _contentSize += _scrollItemInterval + spacing;
            }
        }

        /// <summary>
        ///  Item을 써야할 때 Queue에서 가져와 셋팅해주는 함수<br></br>
        ///  Data를 할당해주는 역할
        /// </summary>
        protected void GetScrollItem(int index, Vector2 pos)
        {
            if (scrollItemList[index] == null)
            {
                var scrollItem = scrollItemQueue.Dequeue();
                scrollItem.SetActive(true);
                scrollItem.InitializeItemData(scrollItemDataList[index]);
                scrollItem.UpdatePos(pos);

                scrollItemList[index] = scrollItem;
            }
        }

        /// <summary>
        /// 쓰지 않는 Item을 Queue에 넣어주는 작업
        /// </summary>
        /// <param name="index"></param>
        protected void ReleaseScrollItem(int index)
        {
            if (scrollItemList[index] != null)
            {
                var scrollItem = scrollItemList[index];
                scrollItemList[index] = null;
                scrollItemQueue.Enqueue(scrollItem);
                scrollItem.SetActive(false);
            }
        }

        /// <summary>
        /// Item과 Data 초기화
        /// </summary>
        protected void Clear()
        {
            for (int i = 0; i < scrollItemList.Count; i++)
            {
                ReleaseScrollItem(i);
            }

            scrollItemList.Clear();
            scrollItemDataList.Clear();
        }

        /// <summary>
        /// Data가 수정됐을 때 갱신하기 위해 만들었음
        /// </summary>
        [ContextMenu("RefreshData")]
        public virtual void RefreshData()
        {
            for (int i = 0; i < scrollItemList.Count; i++)
            {
                scrollItemList[i]?.UpdateData();
            }

            OnValueChanged(normalizedPosition);
        }
    }
}
