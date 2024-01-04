using System;
using UnityEngine;

namespace MyInfinityScrollUI
{
    /// <summary>
    /// -<see cref="InfinityScroll"/>
    /// <br></br><br></br>
    /// ScrollItem이 가지고 있어야 할 Base Data<br></br>
    /// <see cref="ScrollItemBase"/>에서 사용중
    /// </summary>
    [Serializable]
    public class ScrollItemDataBase
    {
        [Header(nameof(ScrollItemDataBase))]
        public int Index;
    }
}