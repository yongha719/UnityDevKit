using System;
using UnityEngine;

namespace MyInfinityScrollUI
{
    /// <summary>
    /// -<see cref="InfinityScroll"/>
    /// <br></br><br></br>
    /// ContentItem이 상속받아야 할 Base Class
    /// </summary>
    [Serializable]
    public class ScrollItemBase : MonoBehaviour
    {
        [Header(nameof(ScrollItemBase))]
        public RectTransform rectTransform;

        public Vector2 Size
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = (RectTransform)transform;

                return rectTransform.rect.size;
            }
        }

        protected virtual void Awake()
        {
        }

        /// <summary>
        /// 인자로 받은 데이터를 할당해줌
        /// </summary>
        public virtual void InitializeItemData(object itemData)
        {
            UpdateData();
        }

        /// <summary>
        /// Data가 바뀌었을 때 해야할 이벤트들
        /// </summary>
        public virtual void UpdateData()
        {
        }

        /// <summary>
        /// Item의 Position을 바꿔줌<br></br>
        /// 호출 - <see cref="InfinityScroll.GetScrollItem(int, Vector2)"/>
        /// </summary>
        /// <param name="pos"></param>
        public void UpdatePos(Vector2 pos)
        {
            rectTransform.anchoredPosition = pos;
        }
    }
}
