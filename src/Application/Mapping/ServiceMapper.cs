//using MapsterMapper;

namespace Application.Mapping
{
    /// <summary>
    /// Thin wrapper around Mapster's IMapper to decouple the application from Mapster types
    /// and expose a simple IMapper interface for the application.
    /// </summary>
    public class ServiceMapper : IMapper
    {
        private readonly MapsterMapper.IMapper _mapper;

        public ServiceMapper(MapsterMapper.IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public TDestination Map<TDestination>(object source)
        {
            if (source == null) return default!;
            return _mapper.Map<TDestination>(source);
        }
    }
}
