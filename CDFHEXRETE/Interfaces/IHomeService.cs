using CDFHEXRETE.Common;
using CDFHEXRETE.DTOS.ReqDTOs;
using CDFHEXRETE.DTOS.ResDTOs;

namespace CDFHEXRETE.Interfaces
{
    public interface IHomeService
    {
        /// <summary>
        /// 查詢匯率日期
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<ResponseData<List<ResHomeQuery>>> GetHomeDatesAsync(ReqHomeQuery req);

        /// <summary>
        /// 取得此日期所有匯率
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<ResponseData<List<ResHomeRate>>> GetHomeDateRatesAsync(ReqHomeDate req);

        /// <summary>
        /// 匯出Excel
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<byte[]> ExportExcelAsync(List<ResHomeRate> req);
    }
}
