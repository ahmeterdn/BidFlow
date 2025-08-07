using BidFlow.Common;
using BidFlow.DTOs.Common;
using System.Linq.Expressions;

namespace BidFlow.Services
{
    public interface IBaseService<TEntity, TResponseDto, TCreateDto, TUpdateDto>
        where TEntity : class
        where TResponseDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<Result<TResponseDto>> GetByIdAsync(int id);
        Task<Result<IEnumerable<TResponseDto>>> GetAllAsync();
        Task<PaginatedResult<TResponseDto>> GetPagedAsync(PaginationRequestDto request);
        Task<Result<TResponseDto>> CreateAsync(TCreateDto createDto);
        Task<Result<TResponseDto>> UpdateAsync(int id, TUpdateDto updateDto);
        Task<Result> DeleteAsync(int id);
        Task<Result<bool>> ExistsAsync(int id);
        Task<Result<IEnumerable<TResponseDto>>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
