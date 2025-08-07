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

        public AdminUsersController(
            IUserService userService,
            IValidator<CreateUserDto> createUserValidator,
            IValidator<UpdateUserDto> updateUserValidator)
        {
            _userService = userService;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] PaginationRequestDto request)
        {
            var result = await _userService.GetPagedAsync(request);
            return result.ToActionResult();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await _userService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return result.ToNotFoundResult();
            }

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var validationResult = await _createUserValidator.ValidateAsync(createUserDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var result = Result<CreateUserDto>.Failure(ErrorMessages.ValidationFailed, errors);
                return result.ToActionResult();
            }

            var createResult = await _userService.CreateAsync(createUserDto);

            if (createResult.IsSuccess)
            {
                return createResult.ToCreatedResult($"/api/admin/adminusers/{createResult.Data?.Id}");
            }

            return createResult.ToActionResult();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                var result = Result<UpdateUserDto>.Failure(ErrorMessages.ValidationFailed, errors);
                return result.ToActionResult();
            }

            var updateResult = await _userService.UpdateAsync(id, updateUserDto);

            if (!updateResult.IsSuccess && updateResult.Message == ErrorMessages.NotFound)
            {
                return updateResult.ToNotFoundResult();
            }

            return updateResult.ToActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleteResult = await _userService.DeleteAsync(id);

            if (!deleteResult.IsSuccess && deleteResult.Message == ErrorMessages.NotFound)
            {
                return deleteResult.ToNotFoundResult();
            }

            return deleteResult.ToActionResult();
        }

        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            var activateResult = await _userService.ActivateUserAsync(id);

            if (!activateResult.IsSuccess && activateResult.Message == ErrorMessages.UserNotFound)
            {
                return activateResult.ToNotFoundResult();
            }

            return activateResult.ToActionResult();
        }

        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var deactivateResult = await _userService.DeactivateUserAsync(id);

            if (!deactivateResult.IsSuccess && deactivateResult.Message == ErrorMessages.UserNotFound)
            {
                return deactivateResult.ToNotFoundResult();
            }

            return deactivateResult.ToActionResult();
        }

        [HttpGet("check-username/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var result = await _userService.IsUsernameExistsAsync(username);
            return result.ToActionResult();
        }

        [HttpGet("check-email/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var result = await _userService.IsEmailExistsAsync(email);
            return result.ToActionResult();
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

            var result = await _userService.GetPagedAsync(request);
            return result.ToActionResult();
        }
    }
}
