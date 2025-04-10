

namespace CDFHEXRATE.Repository.Models
{
    public partial class ExRate
    {
        /// <summary>
        /// 國別
        /// </summary>
       /// public string CountryShortName { get; set; } = null!;

        /// <summary>
        /// 資料日期
        /// </summary>
        public DateTime DataDate { get; set; }

        /// <summary>
        /// 類型: 1早盤、2收盤
        /// </summary>
        public string ExRateType { get; set; } = null!;

        /// <summary>
        /// 交易國別
        /// </summary>
        public string ReferenceCurrency { get; set; } = null!;

        /// <summary>
        /// 台幣報價買入
        /// </summary>
        public decimal LocalBidExRate { get; set; }

        /// <summary>
        /// 台幣報價賣出
        /// </summary>
        public decimal LocalOfferExRate { get; set; }

        /// <summary>
        /// 台幣報價作帳
        /// </summary>
        public decimal LocalSettleExRate { get; set; }

        /// <summary>
        /// 美元報價買入
        /// </summary>
        public decimal USDBidExRate { get; set; }

        /// <summary>
        /// 美元報價賣出
        /// </summary>
        public decimal USDOfferExRate { get; set; }

        /// <summary>
        /// 美元報價作帳
        /// </summary>
        public decimal USDSettleExRate { get; set; }

        /// <summary>
        /// 排程寫入時間
        /// </summary>
        public DateTime SyncTime { get; set; }
    }
}


