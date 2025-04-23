using CDFHEXRATE.Repository.Models;
using CDFHEXRETE.Common;
using CDFHEXRETE.Interfaces;
using CDFHEXRETE.Models;
using CDFHEXRETE.Models.Config;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace CDFHEXRETE.Services
{
    public class ScheduleRateService : IScheduleRateService
    {
        private readonly ILogger<ScheduleRateService> _logger;
        private readonly IConfiguration _config;
        private readonly IAAMService _aamService;

        public ScheduleRateService(
            ILogger<ScheduleRateService> logger,
            IConfiguration config,
            IAAMService aamService)
        {
            _logger = logger;
            _config = config;
            _aamService = aamService;
        }

        private async Task<string> GetExRateDbConnection()
        {
            try
            {
                // 使用原有的加密連線字串方式
                var connectionString = SystemShared.BuildConnectionString("DbConnect");
                return connectionString;
            }
            catch (Exception ex)
            {
                _logger.LogError($"取得 cdfh_exrate 連線字串失敗: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 取得連線至DB-[FHMDDB]的連線資訊
        /// </summary>
        /// <returns></returns>
        public async Task<ConnectAccountData> GetConnectAccountDataAsync()
        {
            var datas = new ConnectAccountData();
            try
            {
                // 非同步方法的同步包裝，直接呼叫不傳入參數
                var aamInfo = Task.Run(async () => await _aamService.Get()).Result;
                if (aamInfo == null)
                {
                    _logger.LogError("取得連線至DB-[FHMDDB]的連線資訊::失敗::無法取得 AAM 資訊");
                    throw new Exception("無法取得 AAM 資訊");
                }

                // 設定連線資訊
                datas.ServerName = aamInfo.Address; // 從 AAM 取得地址
                datas.DatabaseName = "FHMDDB"; // 固定資料庫名稱
                datas.ConnectAccount = aamInfo.UsrName; // 從 AAM 取得使用者名稱
                datas.Password = aamInfo.Content; // 從 AAM 取得密碼

                _logger.LogInformation(@$"取得連線至DB-[FHMDDB]的連線資訊::成功::取得資訊::
                                ServerName: {datas.ServerName}，
                                DatabaseName: {datas.DatabaseName}，
                                ConnectAccount: {datas.ConnectAccount}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"取得連線至DB-[FHMDDB]的連線資訊::錯誤:{ex.Message}");
            }
            return datas;
        }
        public ConnectAccountData GetConnectAccountData()
        {
            return GetConnectAccountDataAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// 取得當日所有匯率
        /// </summary>
        /// <param name="connectData"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ExRate>> GetAllExRateData(ConnectAccountData connectData, string date)
        {
            try
            {
                // 建立連線字串
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = $"server={connectData.ServerName};user={connectData.ConnectAccount};password={connectData.Password};database={connectData.DatabaseName}";
                builder.IntegratedSecurity = false;
                builder.TrustServerCertificate = true;
                string allExRateconnectionString = builder.ToString();

                // 取得當日所有匯率
                _logger.LogInformation(@$"取得/更新當日所有匯率::傳入參數::
                                                    CountryShortName: {_config["CountryShortName"]}，
                                                    date: {date}");

                using (var conn = new SqlConnection(allExRateconnectionString))
                {
                    var sql = @$"EXEC usp_GetAllExRateData @CountryShortName, @date, @type, @striReferenceCurrency";
                    var openDatas = await conn.QueryAsync<ExRate>(sql, new
                    {
                        CountryShortName = _config["CountryShortName"],
                        date = date, // 資料日期，必要輸入{格式：YYYY/MM/DD}
                        type = "1", // 匯率報價時點，必要輸入{'1'-09:30報價(早盤)、'2'-16:35報價(收盤)}
                        striReferenceCurrency = "" // 被報價幣{可指定幣別，若放''表示全部被報價幣均回傳}
                    });
                    var closeDatas = await conn.QueryAsync<ExRate>(sql, new
                    {
                        CountryShortName = _config["CountryShortName"],
                        date = date, // 資料日期，必要輸入{格式：YYYY/MM/DD}
                        type = "2", // 匯率報價時點，必要輸入{'1'-09:30報價(早盤)、'2'-16:35報價(收盤)}
                        striReferenceCurrency = "" // 被報價幣{可指定幣別，若放''表示全部被報價幣均回傳}
                    });

                    var result = openDatas.Union(closeDatas);
                    _logger.LogInformation($"取得當日所有匯率::成功::共 {result.Count()} 筆");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"取得當日所有匯率::錯誤:{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 更新當日所有匯率
        /// </summary>
        /// <param name="listRate"></param>
        /// <returns></returns>
        public async Task UpdateAllExRateData(List<ExRate> listRate)
        {
            try
            {
                // 更新匯率資料            
                var connectionString = await GetExRateDbConnection();
                SqlConnection conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                foreach (var item in listRate)
                {
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = $@"
                        IF EXISTS(SELECT *
                                    FROM ExRate 
                                     WHERE DataDate = @DataDate
                                     AND ExRateType = @ExRateType
                                     AND ReferenceCurrency = @ReferenceCurrency)
                                BEGIN
                                -- 更新
                                    UPDATE ExRate
                                       SET LocalBidExRate = @LocalBidExRate,
                                           LocalOfferExRate = @LocalOfferExRate,
                                           LocalSettleExRate = @LocalSettleExRate,
                                           USDBidExRate = @USDBidExRate,
                                           USDOfferExRate = @USDOfferExRate,
                                           USDSettleExRate = @USDSettleExRate,
                                           SyncTime = @SyncTime
                                       WHERE DataDate = @DataDate
                                       AND ExRateType = @ExRateType
                                       AND ReferenceCurrency = @ReferenceCurrency
                                END
                        ELSE 
                                BEGIN
                                -- 新增
                                     INSERT INTO ExRate(
                                         CountryShortName, DataDate, ExRateType, ReferenceCurrency, 
                                         LocalBidExRate, LocalOfferExRate, LocalSettleExRate, USDBidExRate, USDOfferExRate, USDSettleExRate, SyncTime)
                                     VALUES(
                                         @CountryShortName, @DataDate, @ExRateType, @ReferenceCurrency, 
                                         @LocalBidExRate, @LocalOfferExRate, @LocalSettleExRate, @USDBidExRate, @USDOfferExRate, @USDSettleExRate, @SyncTime)
                                END
                    ";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@CountryShortName", SqlDbType.VarChar).Value = _config["CountryShortName"]; // 國別
                    cmd.Parameters.Add("@DataDate", SqlDbType.Date).Value = item.DataDate; // 資料日期
                    cmd.Parameters.Add("@ExRateType", SqlDbType.VarChar).Value = item.ExRateType; // 類型: 1早盤、2收盤
                    cmd.Parameters.Add("@ReferenceCurrency", SqlDbType.VarChar).Value = item.ReferenceCurrency; // 交易國別
                    cmd.Parameters.Add("@LocalBidExRate", SqlDbType.VarChar).Value = item.LocalBidExRate; // 台幣報價買入
                    cmd.Parameters.Add("@LocalOfferExRate", SqlDbType.VarChar).Value = item.LocalOfferExRate; // 台幣報價賣出
                    cmd.Parameters.Add("@LocalSettleExRate", SqlDbType.VarChar).Value = item.LocalSettleExRate; // 台幣報價作帳
                    cmd.Parameters.Add("@USDBidExRate", SqlDbType.VarChar).Value = item.USDBidExRate; // 美元報價買入
                    cmd.Parameters.Add("@USDOfferExRate", SqlDbType.VarChar).Value = item.USDOfferExRate; // 美元報價賣出
                    cmd.Parameters.Add("@USDSettleExRate", SqlDbType.VarChar).Value = item.USDSettleExRate; // 美元報價作帳
                    cmd.Parameters.Add("@SyncTime", SqlDbType.DateTime).Value = DateTime.Now; // 排程寫入時間
                    await cmd.ExecuteNonQueryAsync();
                }
                _logger.LogInformation("更新當日所有匯率::成功::");
            }
            catch (Exception ex)
            {
                _logger.LogError($"更新當日所有匯率::錯誤:{ex.Message}");
            }
        }
    }
}
