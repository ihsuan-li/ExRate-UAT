

namespace CDFHEXRATE.Repository.Models
{
    public class ConnectAccountData
    {
        /// <summary>
        /// 伺服器名稱
        /// </summary>
        public string ServerName { get; set; } = null!;

        /// <summary>
        /// SQL連結資料庫
        /// </summary>
        public string DatabaseName { get; set; } = null!;

        /// <summary>
        /// SQL使用者名稱
        /// </summary>
        public string ConnectAccount { get; set; } = null!;

        /// <summary>
        /// SQL使用者密碼
        /// </summary>
        public string Password { get; set; } = null!;
    }

}
