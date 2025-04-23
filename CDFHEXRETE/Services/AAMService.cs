using CDFHEXRETE.Interfaces;
using CDFHEXRETE.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CDFHEXRETE.Services
{
    /// <summary>
    /// AAM 服務實作
    /// </summary>
    public class AAMService : IAAMService
    {
        private readonly ILogger<AAMService> _logger;
        private readonly HttpClient _httpClient;

        // AAM 連線資訊直接定義在服務中
        private readonly string _aamUrl = "https://haamt1.cdfhc.com/AIMWebService/api/Accounts";
        private readonly string _appId = "EXR";
        private readonly string _safeId = "EXR_SAFE";
        private readonly string _objectId = "EXR_DB_NSUser_hdbrd21";

        public AAMService(ILogger<AAMService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// 取得 AAM 資訊
        /// </summary>
        /// <returns>AAM 資訊</returns>
        public async Task<AAMInfo> Get()
        {
            var aamInfo = new AAMInfo();
            try
            {
                var apiUrl = $"{_aamUrl}?AppID={_appId}&Query=Safe={_safeId};Object={_objectId}";
                _logger.LogInformation($"AAM URL: {apiUrl}");

                string jsonResult = await GetJsonFromCyberArkApi(apiUrl);
                if (string.IsNullOrWhiteSpace(jsonResult))
                {
                    _logger.LogError("Failed to retrieve JSON data from the CyberArk API.");
                    return aamInfo;
                }

                aamInfo = SetInfo(jsonResult);
                _logger.LogInformation("AAM 取得資料成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAMService.Get");
            }
            return aamInfo;
        }

        /// <summary>
        /// 從 API 取得 JSON 資料
        /// </summary>
        /// <param name="apiUrl">API URL</param>
        /// <returns>JSON 字串</returns>
        private async Task<string> GetJsonFromCyberArkApi(string apiUrl)
        {
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    _logger.LogError($"API request failed with status code: {response.StatusCode}");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetJsonFromCyberArkApi");
                return string.Empty;
            }
        }

        /// <summary>
        /// 設定 AAM 資訊
        /// </summary>
        /// <param name="jsonResult">JSON 字串</param>
        /// <returns>AAM 資訊</returns>
        private AAMInfo SetInfo(string jsonResult)
        {
            var aamInfo = new AAMInfo();
            try
            {
                var jsonArray = JsonConvert.DeserializeObject<JObject>(jsonResult);
                if (jsonArray == null)
                {
                    _logger.LogError("Json 解析失敗");
                    return aamInfo;
                }

                aamInfo.PPPPP = jsonArray["Content"].ToString();
                aamInfo.Content = jsonArray["Content"].ToString();
                aamInfo.UsrName = jsonArray["UserName"].ToString();
                aamInfo.Address = jsonArray["Address"].ToString();
                aamInfo.Name = jsonArray["Name"].ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AAMService.SetInfo");
            }
            return aamInfo;
        }
    }
}
