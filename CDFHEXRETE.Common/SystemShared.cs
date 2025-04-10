using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration.Json;
using CDFHEXRETE.Common.Repository;
using CDFHEXRETE.Common.Extensions;

namespace CDFHEXRETE.Common
{
    /// <summary>
    /// 
    /// </summary>
    ///<remarks>系統共用靜態類別</remarks>
    public static class SystemShared
    {
        /// <summary>
        /// 定義具有索引鍵/值組態屬性
        /// </summary>
        private static IConfiguration configurationEnv { get; set; }
        private static IConfiguration configuration { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        static SystemShared()
        {
            // ReloadOnChange = true; //當appsettings.json被修改時重新加載            

            // 取得開發環境變數，決定用哪一種配置檔
            // 取得專案環境變數
            //var Evironment_Name = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = $"appsettings.json", ReloadOnChange = true })
            .Build();
            var Evironment_Name = configuration.GetValue<string>("ENV") ?? throw new ArgumentNullException("ENV"); // 取環境名稱
            configurationEnv = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = $"appsettings.{Evironment_Name}.json", ReloadOnChange = true })
            .Build();
        }

        public static string dbName;
        public static DatabaseConfigDetail[] databaseConfigDetails;
        public static string providerString;
        public static void Init()
        {
            //var Evironment_Name = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var Evironment_Name = configuration.GetValue<string>("ENV") ?? throw new ArgumentNullException("ENV"); // 取環境名稱
            // 統一用系統環境變數載入對應的DB連線
            var aesKey = configuration.GetValue<string>("AES_KEY") ?? throw new ArgumentNullException("AES_KEY");
            var ivKey = configuration.GetValue<string>("IV_KEY") ?? throw new ArgumentNullException("IV_KEY");

            configurationEnv = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = $"appsettings.{Evironment_Name}.json", ReloadOnChange = true })
            .AddEncryptConfProvider($"appsettings.{Evironment_Name}.json", aesKey, ivKey, true)
            .Build();
            databaseConfigDetails = configurationEnv.GetSection("Database").Get<DatabaseConfigDetail[]>()!;
        }

        /// <summary>
        /// 格式化SQL連線資訊字串
        /// </summary>
        /// <remarks>變更成AES256加密因調整連線字串</remarks>
        /// <returns>Server完整連線字串</returns>
		public static string BuildConnectionString(string dbName)
        {
            foreach (var item in databaseConfigDetails)
            {
                if (item.DbName.ToString() == dbName)
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.ConnectionString = item.ConnectionString;
                    builder.IntegratedSecurity = false;

                    builder.TrustServerCertificate = true;

                    // Build the SqlConnection connection string.
                    providerString = builder.ToString();

                    return providerString;
                }
            }
            return String.Empty;
        }

    }
}