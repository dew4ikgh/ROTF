namespace ROTF.Server.Common
{
    public enum ServiceResultType
    {
        Success,
        NotFound,
        Conflict,
        ValidationError,
        Unauthorized
    }

    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ServiceResultType ResultType { get; set; }

        public static ServiceResponse<T> Ok(T data, string message = "Success") =>
            new() { Data = data, Success = true, ResultType = ServiceResultType.Success, Message = message };

        public static ServiceResponse<T> Fail(ServiceResultType type, string message) =>
            new() { Data = default, Success = false, ResultType = type, Message = message };
    }
}