using FamilyTree.API.Errors;

namespace FamilyTree.API.Responses
{
    public record ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? Message { get; init; } = string.Empty!;
        public ErrorCode? ErrorCode { get; init; }
        public object? Errors { get; init; }

        private ApiResponse(bool success, T? data, string message)
        {
            Success = success;
            Data = data;
            Message = message;
        }
        
        private ApiResponse(bool success, T? data, string message, ErrorCode errorCode, object? errors)
        {
            Success = success; 
            Data = data; 
            Message = message;
            ErrorCode = errorCode;
            Errors = errors;
        }

        public static ApiResponse<T> Ok(T? data, string message) => new(true, data, message);
        public static ApiResponse<T> Error(string message, ErrorCode errorCode, object? errors) => new(false, default, message, errorCode, errors);
    }
}
