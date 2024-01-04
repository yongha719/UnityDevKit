using System.Collections.Generic;

namespace MyInfinityScrollUI
{
    // (Game System & Table Data) -> UI -> Infinite Scroll -> Scroll Item

    /// <summary>
    /// <see cref="InfinityScroll"/>를 상속받음<br></br>
    /// 
    /// <see cref="NameScrollItem"/>를 관리하는 스크롤
    /// </summary>
    public class IS_NameScroll : InfinityScroll
    {
        public void InitializeData(List<NameItemData> colorAndNameContentItemDatas)
        {
            // 초기 데이터 세팅 
            base.InitializeData(colorAndNameContentItemDatas);
        }

        public override void RefreshData()
        {
            base.RefreshData();
        }
    }
}