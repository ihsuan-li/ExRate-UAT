using CDFHEXRETE.Services;
using Coravel.Invocable;

namespace CDFHEXRETE.Invocables
{
    public class GetRateJob : IInvocable
    {
        private readonly ScheduleRateService _scheduleRateService;
        private readonly ILogger<GetRateJob> _logger;

        public GetRateJob(ScheduleRateService scheduleRateService, ILogger<GetRateJob> logger)
        {
            _scheduleRateService = scheduleRateService;
            _logger = logger;
        }
        public async Task Invoke()
        {
            try
            {
                var ConnectAccountData = _scheduleRateService.GetConnectAccountData();
                var allRateDatas = await _scheduleRateService.GetAllExRateData(ConnectAccountData, DateTime.Now.ToString("yyyy/MM/dd"));
                await _scheduleRateService.UpdateAllExRateData(allRateDatas.ToList());
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Exception at GetRateJob.Invoke()");
            }
        }
    }
}
