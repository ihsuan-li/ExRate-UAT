using CDFHEXRETE.Models;
using System.Threading.Tasks;

namespace CDFHEXRETE.Interfaces
{
    /// <summary>
    /// AAM 服務介面
    /// </summary>
    public interface IAAMService
    {
        /// <summary>
        /// 取得 AAM 資訊
        /// </summary>
        /// <returns>AAM 資訊</returns>
        Task<AAMInfo> Get();
    }
}
