using Microsoft.AspNetCore.Mvc;

namespace BidFlow.Common
{
    public static class ApiResponseExtensions
    {
        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(new
                {
                    success = result.IsSuccess,
                    message = result.Message
                });
            }

            return new BadRequestObjectResult(new
            {
                success = result.IsSuccess,
                message = result.Message,
                errors = result.Errors
            });
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    data = result.Data
                });
            }

            return new BadRequestObjectResult(new
            {
                success = result.IsSuccess,
                message = result.Message,
                errors = result.Errors
            });
        }

        public static IActionResult ToActionResult<T>(this PaginatedResult<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    data = result.Data,
                    pagination = new
                    {
                        pageNumber = result.PageNumber,
                        pageSize = result.PageSize,
                        totalCount = result.TotalCount,
                        totalPages = result.TotalPages,
                        hasPreviousPage = result.HasPreviousPage,
                        hasNextPage = result.HasNextPage
                    }
                });
            }

            return new BadRequestObjectResult(new
            {
                success = result.IsSuccess,
                message = result.Message,
                errors = result.Errors
            });
        }

        public static IActionResult ToAdminActionResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    timestamp = DateTime.UtcNow,
                    serverInfo = new { environment = "development" }
                });
            }

            return new BadRequestObjectResult(new
            {
                success = result.IsSuccess,
                message = result.Message,
                errors = result.Errors,
                timestamp = DateTime.UtcNow
            });
        }

        public static IActionResult ToAdminActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    data = result.Data,
                    timestamp = DateTime.UtcNow,
                    serverInfo = new { environment = "development" }
                });
            }

            return new BadRequestObjectResult(new
            {
                success = result.IsSuccess,
                message = result.Message,
                errors = result.Errors,
                timestamp = DateTime.UtcNow
            });
        }

        public static IActionResult ToAdminActionResult<T>(this PaginatedResult<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    data = result.Data,
                    pagination = new
                    {
                        pageNumber = result.PageNumber,
                        pageSize = result.PageSize,
                        totalCount = result.TotalCount,
                        totalPages = result.TotalPages,
                        hasPreviousPage = result.HasPreviousPage,
                        hasNextPage = result.HasNextPage
                    },
                    timestamp = DateTime.UtcNow,
                    serverInfo = new { environment = "development", action = "paginated_query" }
                });
            }

            return new BadRequestObjectResult(new
            {
                success = result.IsSuccess,
                message = result.Message,
                errors = result.Errors,
                timestamp = DateTime.UtcNow
            });
        }

        public static IActionResult ToCreatedResult<T>(this Result<T> result, string location = "")
        {
            if (result.IsSuccess)
            {
                return new CreatedResult(location, new
                {
                    success = result.IsSuccess,
                    message = result.Message,
                    data = result.Data
                });
            }

            return new BadRequestObjectResult(new
            {
                success = result.IsSuccess,
                message = result.Message,
                errors = result.Errors
            });
        }

        public static IActionResult ToNotFoundResult(this Result result)
        {
            return new NotFoundObjectResult(new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        public static IActionResult ToUnauthorizedResult(this Result result)
        {
            return new UnauthorizedObjectResult(new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }
    }
}
