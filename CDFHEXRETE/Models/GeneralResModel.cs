namespace CDFHEXRETE.Models
{
    /// <summary>
    /// General response for API.
    /// </summary>
    public class GeneralResModel
    {
        public int Status { get; set; }

        public string Message { get; set; } = String.Empty;

        public bool Success { get; set; }
    }
}
