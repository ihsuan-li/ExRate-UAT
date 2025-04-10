using CDFHEXRATE.Repository.Contexts;
using CDFHEXRATE.Repository.Models;
using CDFHEXRETE.Common;
using CDFHEXRETE.DTOS.ReqDTOs;
using CDFHEXRETE.DTOS.ResDTOs;
using CDFHEXRETE.Interfaces;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using NLog;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.Streaming.Values;
using NPOI.XSSF.UserModel;

namespace CDFHEXRETE.Services
{
    public class HomeService : IHomeService
    {
        private readonly ILogger<HomeService> _logger;
        private readonly DBContext _dbContext;

        public HomeService(ILogger<HomeService> logger, DBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 查詢匯率日期
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<ResponseData<List<ResHomeQuery>>> GetHomeDatesAsync(ReqHomeQuery req)
        {
            // Dapper 寫法
            var data = new List<ResHomeQuery>();
            try
            {
                using (var conn = _dbContext.Database.GetDbConnection())
                {
                    conn.Open();
                    var sqlQuery = $@" 
                                SELECT DataDate
                                  FROM ExRate
                                 WHERE CONVERT(varchar, DataDate, 111) >= @dateStart
                                   AND CONVERT(varchar, DataDate, 111) <= @dateEnd
                                   AND ExRateType = @ExRateType
				              GROUP BY DataDate
                              ORDER BY DataDate DESC                           
                    ";
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@dateStart", req.dateStart);
                    dynamicParameters.Add("@dateEnd", req.dateEnd);
                    dynamicParameters.Add("@ExRateType", req.ExRateType);
                    var res = await conn.QueryAsync<ResHomeQuery>(sqlQuery, dynamicParameters);
                    data = res.AsQueryable().ToList();
                    var result = new ResponseData<List<ResHomeQuery>>()
                    {
                        RtnCode = 0,
                        Msg = "success",
                        TotalCount = res.Count(),
                        Data = data
                    };
                    return result;
                }
            }
            catch (Exception ex)
            {
                string msg = $"查詢匯率日期::失敗:{ex.Message}";
                _logger.LogError(msg);
                var result = new ResponseData<List<ResHomeQuery>>()
                {
                    RtnCode = 1,
                    Msg = msg,
                    TotalCount = 0,
                    Data = data
                };
                return result;
            }
        }

        /// <summary>
        /// 取得此日期所有匯率
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<ResponseData<List<ResHomeRate>>> GetHomeDateRatesAsync(ReqHomeDate req)
        {
            var data = new List<ResHomeRate>();
            try
            {
                // Dapper 寫法
                using (var conn = _dbContext.Database.GetDbConnection())
                {
                    conn.Open();
                    var sqlQuery = $@" 
                                SELECT *
                                  FROM ExRate
                                 WHERE CONVERT(varchar, DataDate, 111) =CONVERT(varchar, @dataDate, 111) 
                                   AND ExRateType = @ExRateType
                              ORDER BY DataDate DESC                           
                    ";
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@dataDate", req.DataDate);
                    dynamicParameters.Add("@ExRateType", req.ExRateType);
                    var res = await conn.QueryAsync<ResHomeRate>(sqlQuery, dynamicParameters);
                    data = res.AsQueryable().ToList();
                    var result = new ResponseData<List<ResHomeRate>>()
                    {
                        RtnCode = 0,
                        Msg = "success",
                        TotalCount = res.Count(),
                        Data = data
                    };
                    return result;
                }
            }
            catch (Exception ex)
            {
                string msg = $"取得此日期所有匯率::失敗:{ex.Message}";
                _logger.LogError(msg);
                var result = new ResponseData<List<ResHomeRate>>()
                {
                    RtnCode = 1,
                    Msg = msg,
                    TotalCount = 0,
                    Data = data
                };
                return result;
            }
        }

        /// <summary>
        /// 匯出Excel
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<byte[]> ExportExcelAsync(List<ResHomeRate> data)
        {
            try
            {
                #region 手工建立
                //// 建立工作表
                //var workbook = new XSSFWorkbook();
                //XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet("匯率");

                //// 定義欄位名稱列
                //string[] columnNameArray = { "幣別", "", "買入", "賣出", "作帳", "買入", "賣出", "作帳" };

                ////設定顏色
                //ICellStyle mergeCellStyle = workbook.CreateCellStyle();
                ////字體顏色
                //IFont font = (XSSFFont)workbook.CreateFont();
                //font.Color = NPOI.SS.UserModel.IndexedColors.Red.Index;
                //mergeCellStyle.SetFont(font);

                //int row = 0;
                //// 建立第一行設定表頭資料
                //sheet.CreateRow(row);
                //DateTime dataDate = DateTime.Parse(data[0].DataDate!);
                //sheet.GetRow(row).CreateCell(0).SetCellValue(dataDate.ToString("yyyy/MM/dd"));
                //sheet.GetRow(row).CreateCell(2).SetCellValue("早盤匯率表"); 
                //CellRangeAddress mergeRegion = new CellRangeAddress(0, 0, 2, 7);
                //sheet.AddMergedRegion(mergeRegion); // 合併儲存格

                //// 建立第二行設定表頭資料
                //row++;
                //sheet.CreateRow(row);
                //sheet.GetRow(row).CreateCell(2).SetCellValue("台幣報價");
                //mergeRegion = new CellRangeAddress(1, 1, 2, 4);
                //sheet.AddMergedRegion(mergeRegion); // 合併儲存格
                //sheet.GetRow(row).CreateCell(5).SetCellValue("美元報價");
                //mergeRegion = new CellRangeAddress(1, 1, 5, 7);
                //sheet.AddMergedRegion(mergeRegion); // 合併儲存格

                //// 建立第三行設定表頭資料
                //row++;
                //sheet.CreateRow(row);
                //sheet.GetRow(row).CreateCell(2).SetCellValue("DBU");
                // mergeRegion = new CellRangeAddress(2, 2, 2, 4);
                //sheet.AddMergedRegion(mergeRegion); // 合併儲存格
                //sheet.GetRow(row).CreateCell(5).SetCellValue("OBU");
                // mergeRegion = new CellRangeAddress(2, 2, 5, 7);
                //sheet.AddMergedRegion(mergeRegion); // 合併儲存格
                //sheet.GetRow(row).GetCell(5).CellStyle = mergeCellStyle; // 設定文字顏色
                //// 建立第四行設定表頭資料
                //row++;
                //sheet.CreateRow(row);
                //for (int i = 0; i < columnNameArray.Count(); i++)
                //{
                //    sheet.GetRow(row).CreateCell(i).SetCellValue(columnNameArray[i]);
                //}

                //// 定義幣別順序(固定)
                //Dictionary<string, string> dictCurrency = new Dictionary<string, string>()
                //{
                //    {"TWD", "台幣"},
                //    {"USD", "美元"},
                //    {"JPY", "日圓"},
                //    {"EUR", "歐元"},
                //    {"GBP", "英鎊"},
                //    {"CHF", "瑞朗"},
                //    {"AUD", "澳幣"},
                //    {"NZD", "紐幣"},
                //    {"HKD", "港幣"},
                //    {"CAD", "加幣"},
                //    {"SGD", "坡幣"},
                //    {"THB", "泰銖"},
                //    {"MYR", "馬幣"},
                //    {"PHP", "披索"},
                //    {"IDR", "盧比"},
                //    {"HUF", "匈牙利"},
                //    {"MXN", "墨西哥"},
                //    {"BRL", "巴西"},
                //    {"ZAR", "南非幣"},
                //    {"PLN", "波蘭"},
                //    {"RUB", "蘇俄"},
                //    {"CNY", "人民幣"},
                //    {"KRW", "韓圜"},
                //    {"INR", "盧布"},
                //    {"NOK", "挪威幣"} ,
                //    {"SEK", "瑞典幣"} ,
                //    {"CZK", "捷克克朗"},
                //    {"VND", "越南盾"},
                //    {"CNH", "境外人民幣"}
                //};

                //foreach (var item in dictCurrency)
                //{
                //    // 取得幣別對應的匯率資料
                //    var datarow = data.Where(x => x.ReferenceCurrency == item.Key).FirstOrDefault();

                //    row++;
                //    IRow rowtemp = sheet.CreateRow(row);
                //    int i = -1;
                //    i++;
                //    rowtemp.CreateCell(i).SetCellValue(item.Key);
                //    i++;
                //    rowtemp.CreateCell(i).SetCellValue(item.Value);
                //    i++;
                //    rowtemp.CreateCell(i).SetCellValue(DeleteZero(datarow?.LocalBidExRate) ?? "0");
                //    i++;
                //    rowtemp.CreateCell(i).SetCellValue(DeleteZero(datarow?.LocalOfferExRate) ?? "0");
                //    i++;
                //    rowtemp.CreateCell(i).SetCellValue(DeleteZero(datarow?.LocalSettleExRate) ?? "0");
                //    i++;
                //    rowtemp.CreateCell(i).SetCellValue(DeleteZero(datarow?.USDBidExRate) ?? "0");
                //    i++;
                //    rowtemp.CreateCell(i).SetCellValue(DeleteZero(datarow?.USDOfferExRate) ?? "0");
                //    i++;
                //    rowtemp.CreateCell(i).SetCellValue(DeleteZero(datarow?.USDSettleExRate) ?? "0");
                //}

                //int rownum = sheet.PhysicalNumberOfRows;
                //for (int i = 3; i < rownum; i++)
                //{
                //    sheet.GetRow(i).GetCell(5).CellStyle = mergeCellStyle; // 設定文字顏色
                //    sheet.GetRow(i).GetCell(6).CellStyle = mergeCellStyle; // 設定文字顏色
                //    sheet.GetRow(i).GetCell(7).CellStyle = mergeCellStyle; // 設定文字顏色
                //}
                //// 設定欄寬
                //sheet.SetColumnWidth(0, 4 * 256); 
                //sheet.SetColumnWidth(2, 5 * 256);
                #endregion 手工建立

                #region 使用模板
                // 使用模板
                var ms = new NpoiMemoryStream();
                // 實體路徑的excel檔 = 應用程式所在的目錄 + 所在頁面
                string path = AppDomain.CurrentDomain.BaseDirectory + @"ExcelTemplate\templateEXRate.xlsx";
                using (FileStream templateFile = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new XSSFWorkbook(templateFile);
                    ISheet sheet = workbook.GetSheetAt(0);

                    string typeName = data[0].ExRateType == "1" ? "早盤" : "收盤";

                    int row = 0;
                    // 建立第一行設定表頭資料
                    DateTime dataDate = DateTime.Parse(data[0].DataDate!);
                    sheet.GetRow(row).GetCell(0).SetCellValue(dataDate.ToString("yyyy/MM/dd"));
                    sheet.GetRow(row).GetCell(2).SetCellValue($"{typeName}匯率表");


                    // 定義幣別順序(固定)
                    Dictionary<string, string> dictCurrency = new Dictionary<string, string>()
                {
                    {"TWD", "台幣"},
                    {"USD", "美元"},
                    {"JPY", "日圓"},
                    {"EUR", "歐元"},
                    {"GBP", "英鎊"},
                    {"CHF", "瑞朗"},
                    {"AUD", "澳幣"},
                    {"NZD", "紐幣"},
                    {"HKD", "港幣"},
                    {"CAD", "加幣"},
                    {"SGD", "坡幣"},
                    {"THB", "泰銖"},
                    {"MYR", "馬幣"},
                    {"PHP", "披索"},
                    {"IDR", "盧比"},
                    {"HUF", "匈牙利"},
                    {"MXN", "墨西哥"},
                    {"BRL", "巴西"},
                    {"ZAR", "南非幣"},
                    {"PLN", "波蘭"},
                    {"RUB", "蘇俄"},
                    {"CNY", "人民幣"},
                    {"KRW", "韓圜"},
                    {"INR", "盧布"},
                    {"NOK", "挪威幣"} ,
                    {"SEK", "瑞典幣"} ,
                    {"CZK", "捷克克朗"},
                    {"VND", "越南盾"},
                    {"CNH", "境外人民幣"}
                };

                    row = 3;
                    foreach (var item in dictCurrency)
                    {
                        // 取得幣別對應的匯率資料
                        var datarow = data.Where(x => x.ReferenceCurrency == item.Key).FirstOrDefault();

                        row++;
                        IRow rowtemp = sheet.GetRow(row);
                        int i = -1;
                        i++;
                        rowtemp.GetCell(i).SetCellValue(item.Key);
                        i++;
                        rowtemp.GetCell(i).SetCellValue(item.Value);
                        i++;
                        rowtemp.GetCell(i).SetCellValue(datarow?.LocalBidExRate!??0);
                        rowtemp.GetCell(i).SetCellType(NPOI.SS.UserModel.CellType.Numeric);
                        i++;
                        rowtemp.GetCell(i).SetCellValue(datarow?.LocalOfferExRate! ?? 0);
                        rowtemp.GetCell(i).SetCellType(NPOI.SS.UserModel.CellType.Numeric);
                        i++;
                        rowtemp.GetCell(i).SetCellValue(datarow?.LocalSettleExRate! ?? 0);
                        rowtemp.GetCell(i).SetCellType(NPOI.SS.UserModel.CellType.Numeric);
                        i++;
                        rowtemp.GetCell(i).SetCellValue(datarow?.USDBidExRate! ?? 0);
                        rowtemp.GetCell(i).SetCellType(NPOI.SS.UserModel.CellType.Numeric);
                        i++;
                        rowtemp.GetCell(i).SetCellValue(datarow?.USDOfferExRate! ?? 0);
                        rowtemp.GetCell(i).SetCellType(NPOI.SS.UserModel.CellType.Numeric);
                        i++;
                        rowtemp.GetCell(i).SetCellValue(datarow?.USDSettleExRate! ?? 0);
                        rowtemp.GetCell(i).SetCellType(NPOI.SS.UserModel.CellType.Numeric);
                    }


                    ms.AllowClose = false;
                    workbook.Write(ms);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.AllowClose = true;
                }

                #endregion 使用模板

                return ms.ToArray();
            }
            catch (Exception ex)
            {
                string msg = $"匯出Excel::失敗:{ex.Message}";
                _logger.LogError(msg);
                return null;
            }
        }

        public string DeleteZero(string text)
        {
            decimal value = 0;
            decimal.TryParse(text, out value);
            return value.ToString("0.#####");
        }

        public class NpoiMemoryStream : MemoryStream
        {
            public NpoiMemoryStream()
            {
                AllowClose = true;
            }

            public bool AllowClose { get; set; }

            public override void Close()
            {
                if (AllowClose)
                    base.Close();
            }
        }

    }
}
