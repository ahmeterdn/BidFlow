namespace BidFlow.Common
{
    public class PaginatedResult<T>
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public List<string> Errors { get; private set; } = new();
        public IEnumerable<T>? Data { get; private set; }

        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        private PaginatedResult(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize, string message)
        {
            IsSuccess = true;
            Message = message;
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        private PaginatedResult(string message, List<string> errors)
        {
            IsSuccess = false;
            Message = message;
            Errors = errors;
            Data = Enumerable.Empty<T>();
        }

        public static PaginatedResult<T> Success(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize,
            string message = "Data retrieved successfully")
        {
            return new PaginatedResult<T>(data, totalCount, pageNumber, pageSize, message);
        }

        public static PaginatedResult<T> Failure(string message)
        {
            return new PaginatedResult<T>(message, new List<string>());
        }

        public static PaginatedResult<T> Failure(string message, List<string> errors)
        {
            return new PaginatedResult<T>(message, errors);
        }
    }
}
