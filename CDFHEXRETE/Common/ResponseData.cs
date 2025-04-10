namespace CDFHEXRETE.Common
{
    public class ResponseData<T>
    {
        public int RtnCode { get; set; }

        public string Msg { get; set; }

        public int TotalCount { get; set; }

        public T Data { get; set; }
    }
}
