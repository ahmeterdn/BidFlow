namespace BidFlow.Common
{
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; } = string.Empty;
        public List<string> Errors { get; protected set; } = new();

        protected Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        protected Result(bool isSuccess, string message, List<string> errors)
        {
            IsSuccess = isSuccess;
            Message = message;
            Errors = errors ?? new List<string>();
        }

        public static Result Success(string message = "Operation completed successfully")
        {
            return new Result(true, message);
        }

        public static Result Failure(string message)
        {
            return new Result(false, message);
        }

        public static Result Failure(string message, List<string> errors)
        {
            return new Result(false, message, errors);
        }

        public static Result Failure(List<string> errors)
        {
            return new Result(false, "Operation failed", errors);
        }
    }

    public class Result<T> : Result
    {
        public T? Data { get; private set; }

        private Result(bool isSuccess, string message, T? data = default)
            : base(isSuccess, message)
        {
            Data = data;
        }

        private Result(bool isSuccess, string message, List<string> errors, T? data = default)
            : base(isSuccess, message, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data, string message = "Operation completed successfully")
        {
            return new Result<T>(true, message, data);
        }

        public static new Result<T> Success(string message = "Operation completed successfully")
        {
            return new Result<T>(true, message);
        }

        public static new Result<T> Failure(string message)
        {
            return new Result<T>(false, message);
        }

        public static new Result<T> Failure(string message, List<string> errors)
        {
            return new Result<T>(false, message, errors);
        }

        public static new Result<T> Failure(List<string> errors)
        {
            return new Result<T>(false, "Operation failed", errors);
        }

        
        public static implicit operator Result<T>(T data)
        {
            return Success(data);
        }
    }
}
