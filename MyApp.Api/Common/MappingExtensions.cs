using AutoMapper;

namespace BidFlow.Common
{
    public static class MappingExtensions
    {
        public static Result<TDestination> MapTo<TDestination>(this Result<object> source, IMapper mapper)
        {
            if (!source.IsSuccess || source.Data == null)
            {
                return Result<TDestination>.Failure(source.Message, source.Errors);
            }

            try
            {
                var mapped = mapper.Map<TDestination>(source.Data);
                return Result<TDestination>.Success(mapped, source.Message);
            }
            catch (Exception ex)
            {
                return Result<TDestination>.Failure($"Mapping failed: {ex.Message}");
            }
        }

        public static Result<IEnumerable<TDestination>> MapToList<TSource, TDestination>(
            this Result<IEnumerable<TSource>> source, IMapper mapper)
        {
            if (!source.IsSuccess || source.Data == null)
            {
                return Result<IEnumerable<TDestination>>.Failure(source.Message, source.Errors);
            }

            try
            {
                var mapped = mapper.Map<IEnumerable<TDestination>>(source.Data);
                return Result<IEnumerable<TDestination>>.Success(mapped, source.Message);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<TDestination>>.Failure($"Mapping failed: {ex.Message}");
            }
        }

        public static PaginatedResult<TDestination> MapToPaginated<TSource, TDestination>(
            this PaginatedResult<TSource> source, IMapper mapper)
        {
            if (!source.IsSuccess || source.Data == null)
            {
                return PaginatedResult<TDestination>.Failure(source.Message, source.Errors);
            }

            try
            {
                var mapped = mapper.Map<IEnumerable<TDestination>>(source.Data);
                return PaginatedResult<TDestination>.Success(
                    mapped,
                    source.TotalCount,
                    source.PageNumber,
                    source.PageSize,
                    source.Message);
            }
            catch (Exception ex)
            {
                return PaginatedResult<TDestination>.Failure($"Mapping failed: {ex.Message}");
            }
        }
    }
}
