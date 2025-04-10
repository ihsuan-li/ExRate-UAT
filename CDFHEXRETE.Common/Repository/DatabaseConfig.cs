

namespace CDFHEXRETE.Common.Repository
{
    public class DatabaseConfig
    {
        public ICollection<DatabaseConfigDetail> Details { get; set; } = new List<DatabaseConfigDetail>();
    }

    /// <summary>appsettings for Database Section，主要用於將 appsetting 資料庫設定區塊轉換為物件</summary>
    public class DatabaseConfigDetail
    {
        public DBName DbName { get; set; }

        public string ConnectionString { get; set; } = string.Empty;
    }
}
