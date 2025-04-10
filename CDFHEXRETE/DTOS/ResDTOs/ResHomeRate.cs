namespace CDFHEXRETE.DTOS.ResDTOs
{
    public class ResHomeRate
    {
        /// <summary>
        /// 國別
        /// </summary>
        /// public string? CountryShortName { get; set; }

        /// <summary>
        /// 資料日期
        /// </summary>
        public string? DataDate { get; set; }

        /// <summary>
        /// 類型: 1早盤、2收盤
        /// </summary>
        public string? ExRateType { get; set; }

        /// <summary>
        /// 交易國別
        /// </summary>
        public string? ReferenceCurrency { get; set; }

        /// <summary>
        /// 台幣報價買入
        /// </summary>
        public double LocalBidExRate { get; set; }

        /// <summary>
        /// 台幣報價賣出
        /// </summary>
        public double LocalOfferExRate { get; set; }

        /// <summary>
        /// 台幣報價作帳
        /// </summary>
        public double LocalSettleExRate { get; set; }

        /// <summary>
        /// 美元報價買入
        /// </summary>
        public double USDBidExRate { get; set; }

        /// <summary>
        /// 美元報價賣出
        /// </summary>
        public double USDOfferExRate { get; set; }

        /// <summary>
        /// 美元報價作帳
        /// </summary>
        public double USDSettleExRate { get; set; }

        /// <summary>
        /// 排程寫入時間
        /// </summary>
        public string? SyncTime { get; set; }
    }
}
