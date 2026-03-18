using System;
using Newtonsoft.Json;

namespace SimpleSolitaire.Controller.NativeBridge.Models
{
    /// <summary>
    /// 提现接口参数模型（适配 Solitaire 游戏数据）
    /// </summary>
    [Serializable]
    public class WithdrawParams
    {
        /// <summary>当前货币数量（金币/奖励积分）</summary>
        [JsonProperty("currentAmount")]
        public string CurrentAmount { get; set; }

        /// <summary>当前金币</summary>
        [JsonProperty("currentCoin")]
        public string CurrentCoin { get; set; }

        /// <summary>当前关卡或游戏进度</summary>
        [JsonProperty("currentLevel")]
        public string CurrentLevel { get; set; }

        /// <summary>看广告次数</summary>
        [JsonProperty("adCount")]
        public string AdCount { get; set; }

        /// <summary>历史最高分</summary>
        [JsonProperty("currentScore")]
        public string CurrentScore { get; set; }

        /// <summary>胜利局数</summary>
        [JsonProperty("gamesWon")]
        public string GamesWon { get; set; }

        /// <summary>创建默认参数实例</summary>
        public static WithdrawParams CreateDefault()
        {
            return new WithdrawParams
            {
                CurrentAmount = "0",
                CurrentCoin   = "0",
                CurrentLevel  = "0",
                AdCount       = "0",
                CurrentScore  = "0",
                GamesWon      = "0"
            };
        }
    }
}
