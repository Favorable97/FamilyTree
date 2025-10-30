namespace FamilyTree.API.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; } = string.Empty!;
        
        private ApiResponse(bool success, T? data, string message)
        {
            Success = success; 
            Data = data; 
            Message = message;
        }

        public static ApiResponse<T> Ok(T? data, string message) => new(true, data, message);
        public static ApiResponse<T> Error(string message) => new(false, default, message);
    }
}
