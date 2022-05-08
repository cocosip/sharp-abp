namespace SharpAbp.MinId
{
    public class MinIdResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static MinIdResponse<T> Successful<T>(T data)
        {
            return new MinIdResponse<T>()
            {
                Success = true,
                Data = data
            };
        }

        public static MinIdResponse<T> Failed<T>(string message)
        {
            return new MinIdResponse<T>()
            {
                Success = false,
                Message = message
            };
        }
    }

    public class MinIdResponse<T> : MinIdResponse
    {
        public T Data { get; set; }
    }
}
