using CDFHEXRATE.Repository.Models;

namespace CDFHEXRETE.Interfaces
{
    public interface IScheduleRateService
    {
        public ConnectAccountData GetConnectAccountData();
        public Task<IEnumerable<ExRate>> GetAllExRateData(ConnectAccountData connectData, string date);
        public Task UpdateAllExRateData(List<ExRate> listRate);
    }
}
