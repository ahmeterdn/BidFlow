using BidFlow.Common;
using BidFlow.DTOs.Common;
using BidFlow.DTOs.User;
using BidFlow.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidFlow.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<CreateUserDto> _createUserValidator;
        private readonly IValidator<UpdateUserDto> _updateUserValidator;
        private readonly IValidator<PaginationRequestDto> _paginationValidator;

        public AdminUsersController(
            IUserService userService,
            IValidator<CreateUserDto> createUserValidator,
            IValidator<UpdateUserDto> updateUserValidator,
            IValidator<PaginationRequestDto> paginationValidator)
        {
            _userService = userService;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
            _paginationValidator = paginationValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationRequestDto request)
        {
            var validationResult = await _paginationValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var result = Result<PaginationRequestDto>.Failure(ErrorMessages.ValidationFailed, errors);
                return result.ToAdminActionResult();
            }

            var getUsersResult = await _userService.GetPagedAsync(request);
            return getUsersResult.ToAdminActionResult();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await _userService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return new NotFoundObjectResult(new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors,
                    timestamp = DateTime.UtcNow
                });
            }

            return result.ToAdminActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var validationResult = await _createUserValidator.ValidateAsync(createUserDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var result = Result<CreateUserDto>.Failure(ErrorMessages.ValidationFailed, errors);
                return result.ToAdminActionResult();
            }

            var createResult = await _userService.CreateUserForAdminAsync(createUserDto);

            if (createResult.IsSuccess)
            {
                return new CreatedResult($"/api/admin/adminusers/{createResult.Data?.Id}", new
                {
                    success = createResult.IsSuccess,
                    message = createResult.Message,
                    data = createResult.Data,
                    timestamp = DateTime.UtcNow,
                    serverInfo = new { environment = "development", action = "user_created" }
                });
            }

            return createResult.ToAdminActionResult();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var result = Result<UpdateUserDto>.Failure(ErrorMessages.ValidationFailed, errors);
                return result.ToAdminActionResult();
            }

            var updateResult = await _userService.UpdateAsync(id, updateUserDto);

            if (!updateResult.IsSuccess && updateResult.Message == ErrorMessages.NotFound)
            {
                return new NotFoundObjectResult(new
                {
                    success = false,
                    message = updateResult.Message,
                    errors = updateResult.Errors,
                    timestamp = DateTime.UtcNow
                });
            }

            return updateResult.ToAdminActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleteResult = await _userService.DeleteAsync(id);

            if (!deleteResult.IsSuccess && deleteResult.Message == ErrorMessages.NotFound)
            {
                return new NotFoundObjectResult(new
                {
                    success = false,
                    message = deleteResult.Message,
                    errors = deleteResult.Errors,
                    timestamp = DateTime.UtcNow
                });
            }

            return deleteResult.ToAdminActionResult();
        }

        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var activateResult = await _userService.ActivateUserAsync(id);

            if (!activateResult.IsSuccess && activateResult.Message == ErrorMessages.UserNotFound)
            {
                return new NotFoundObjectResult(new
                {
                    success = false,
                    message = activateResult.Message,
                    errors = activateResult.Errors,
                    timestamp = DateTime.UtcNow
                });
            }

            return activateResult.ToAdminActionResult();
        }

        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var deactivateResult = await _userService.DeactivateUserAsync(id);

            if (!deactivateResult.IsSuccess && deactivateResult.Message == ErrorMessages.UserNotFound)
            {
                return new NotFoundObjectResult(new
                {
                    success = false,
                    message = deactivateResult.Message,
                    errors = deactivateResult.Errors,
                    timestamp = DateTime.UtcNow
                });
            }

            return deactivateResult.ToAdminActionResult();
        }

        [HttpGet("check-username/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var result = await _userService.IsUsernameExistsAsync(username);
            return result.ToAdminActionResult();
        }

        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var result = await _userService.IsEmailExistsAsync(email);
            return result.ToAdminActionResult();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var request = new PaginationRequestDto
            {
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var validationResult = await _paginationValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var result = Result<PaginationRequestDto>.Failure(ErrorMessages.ValidationFailed, errors);
                return result.ToAdminActionResult();
            }

            var searchResult = await _userService.GetPagedAsync(request);
            return searchResult.ToAdminActionResult();
        }
    }
}
