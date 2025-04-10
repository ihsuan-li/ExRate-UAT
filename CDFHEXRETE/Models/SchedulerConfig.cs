namespace CDFHEXRETE.Models
{
    public class SchedulerConfig
    {
        public bool Enable { get; set; }

        public string Time { get; set; } = "*/5 * * * *";
    }
}
