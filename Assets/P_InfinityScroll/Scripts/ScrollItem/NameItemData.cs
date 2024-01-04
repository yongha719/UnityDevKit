using System;
using UnityEngine;

namespace MyInfinityScrollUI
{
    /// <summary>
    /// -<see cref="InfinityScroll"/>
    /// <br></br><br></br> 
    /// <see cref="ScrollItemDataBase"/>를 상속받음<br></br>
    /// <see cref="NameScrollItem"/>에서 사용<br></br>
    /// </summary>
    [Serializable]
    public class NameItemData : ScrollItemDataBase
    {
        [Header(nameof(NameItemData))]
        public int Count;

        public string Name;

        public Color NameTextColor;
    }
}