using AutoMapper;
using BidFlow.Common;
using BidFlow.DTOs.Common;
using BidFlow.Entities;
using System.Linq.Expressions;

namespace BidFlow.Services
{
    public abstract class BaseService<TEntity, TResponseDto, TCreateDto, TUpdateDto>
        : IBaseService<TEntity, TResponseDto, TCreateDto, TUpdateDto>
        where TEntity : BaseEntity
        where TResponseDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        protected BaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public virtual async Task<Result<TResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);

                if (entity == null)
                {
                    return Result<TResponseDto>.Failure(ErrorMessages.NotFound);
                }

                var dto = _mapper.Map<TResponseDto>(entity);
                return Result<TResponseDto>.Success(dto, SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return Result<TResponseDto>.Failure($"Error retrieving entity: {ex.Message}");
            }
        }

        public virtual async Task<Result<IEnumerable<TResponseDto>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Repository<TEntity>().GetAllAsync();
                var dtos = _mapper.Map<IEnumerable<TResponseDto>>(entities);

                return Result<IEnumerable<TResponseDto>>.Success(dtos, SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<TResponseDto>>.Failure($"Error retrieving entities: {ex.Message}");
            }
        }

        public virtual async Task<PaginatedResult<TResponseDto>> GetPagedAsync(PaginationRequestDto request)
        {
            try
            {
                Expression<Func<TEntity, bool>>? predicate = null;

                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    predicate = BuildSearchPredicate(request.SearchTerm);
                }

                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null;
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    orderBy = BuildOrderBy(request.SortBy, request.SortDirection);
                }

                var pagedEntities = await _unitOfWork.Repository<TEntity>()
                    .GetPagedAsync(request.PageNumber, request.PageSize, predicate, orderBy);

                var totalCount = await _unitOfWork.Repository<TEntity>()
                    .CountAsync(predicate);

                var dtos = _mapper.Map<IEnumerable<TResponseDto>>(pagedEntities);

                return PaginatedResult<TResponseDto>.Success(
                    dtos,
                    totalCount,
                    request.PageNumber,
                    request.PageSize,
                    SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return PaginatedResult<TResponseDto>.Failure($"Error retrieving paged data: {ex.Message}");
            }
        }

        public virtual async Task<Result<TResponseDto>> CreateAsync(TCreateDto createDto)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(createDto);

                var createdEntity = await _unitOfWork.Repository<TEntity>().AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var responseDto = _mapper.Map<TResponseDto>(createdEntity);
                return Result<TResponseDto>.Success(responseDto, SuccessMessages.DataSaved);
            }
            catch (Exception ex)
            {
                return Result<TResponseDto>.Failure($"Error creating entity: {ex.Message}");
            }
        }

        public virtual async Task<Result<TResponseDto>> UpdateAsync(int id, TUpdateDto updateDto)
        {
            try
            {
                var existingEntity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);

                if (existingEntity == null)
                {
                    return Result<TResponseDto>.Failure(ErrorMessages.NotFound);
                }

                _mapper.Map(updateDto, existingEntity);

                _unitOfWork.Repository<TEntity>().Update(existingEntity);
                await _unitOfWork.SaveChangesAsync();

                var responseDto = _mapper.Map<TResponseDto>(existingEntity);
                return Result<TResponseDto>.Success(responseDto, SuccessMessages.DataSaved);
            }
            catch (Exception ex)
            {
                return Result<TResponseDto>.Failure($"Error updating entity: {ex.Message}");
            }
        }

        public virtual async Task<Result> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);

                if (entity == null)
                {
                    return Result.Failure(ErrorMessages.NotFound);
                }

                _unitOfWork.Repository<TEntity>().Delete(entity);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success(SuccessMessages.DataSaved);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error deleting entity: {ex.Message}");
            }
        }

        public virtual async Task<Result<bool>> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.Repository<TEntity>().ExistsAsync(x => x.Id == id);
                return Result<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error checking entity existence: {ex.Message}");
            }
        }

        public virtual async Task<Result<IEnumerable<TResponseDto>>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                var entities = await _unitOfWork.Repository<TEntity>().FindAsync(predicate);
                var dtos = _mapper.Map<IEnumerable<TResponseDto>>(entities);

                return Result<IEnumerable<TResponseDto>>.Success(dtos, SuccessMessages.DataRetrieved);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<TResponseDto>>.Failure($"Error finding entities: {ex.Message}");
            }
        }

        protected abstract Expression<Func<TEntity, bool>>? BuildSearchPredicate(string searchTerm);
        protected abstract Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? BuildOrderBy(string sortBy, string? sortDirection);
    }
}
