using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// Word Solitaire 游戏配置
    /// </summary>
    [CreateAssetMenu(fileName = "WordSolitaireConfig", menuName = "WordSolitaire/GameConfig")]
    public class WordSolitaireConfig : ScriptableObject
    {
        [Header("金币配置")]
        public int InitialCoins = 100;           // 初始金币
        public int NormalLevelReward = 10;       // 普通关卡奖励
        public int MilestoneLevelReward = 50;    // 里程碑关卡奖励
        
        [Header("里程碑关卡")]
        public List<int> MilestoneLevels = new List<int> { 5, 10, 15, 20 }; // 里程碑关卡列表
        
        [Header("道具配置")]
        public int InitialHints = 3;             // 初始提示数量
        public int InitialUndos = 3;             // 初始撤回数量
        public int InitialJokers = 1;            // 初始万能牌数量
        
        [Header("提示配置")]
        public int HintCost = 5;                 // 提示消耗金币
        public int UndoCost = 5;                 // 撤回消耗金币
        public int JokerCost = 10;               // 万能牌消耗金币
    }
}
