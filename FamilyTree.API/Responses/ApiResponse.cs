namespace FamilyTree.API.Responses
{
    public record ApiResponse<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? Message { get; init; } = string.Empty!;
        public string? ErrorCode { get; init; } = string.Empty!;
        
        private ApiResponse(bool success, T? data, string message, string errorCode)
        {
            Success = success; 
            Data = data; 
            Message = message;
            ErrorCode = errorCode;
        }

        public static ApiResponse<T> Ok(T? data, string message) => new(true, data, message, "");
        public static ApiResponse<T> Error(string message, string errorCode) => new(false, default, message, errorCode);
    }
}
