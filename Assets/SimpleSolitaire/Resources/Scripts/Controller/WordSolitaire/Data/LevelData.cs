using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 关卡配置数据
    /// </summary>
    [CreateAssetMenu(fileName = "Level_New", menuName = "WordSolitaire/LevelData")]
    public class LevelData : ScriptableObject
    {
        [Header("基础信息")]
        public int LevelId;                                    // 关卡ID
        public int CardCount;                                  // 卡牌总数
        public int MaxMoves;                                   // 最大步数（999表示无限）
        public int ColumnCount;                                // 列区数量
        public int SlotCount;                                  // 分类槽数量
        
        [Header("类别配置")]
        public string[] CategoryIds;                          // 涉及的类别
        
        [Header("其他设置")]
        public bool IsTutorial;                                // 是否引导关卡
        public bool IsShowResultAd;                            // 关卡结算时是否播放插屏广告
        public bool IsShowMatchAd;                             // 第一个分类卡集齐时是否播放插屏广告
    }
}
