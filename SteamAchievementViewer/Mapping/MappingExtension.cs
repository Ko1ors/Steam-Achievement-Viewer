using AutoMapper;

namespace SteamAchievementViewer.Mapping
{
    public static class MappingExtension
    {
        public static T MapMultiple<T>(this IMapper mapper, object firstSource, object secondSource)
        {
            return mapper.Map(secondSource, mapper.Map<T>(firstSource));
        }

        public static T MapMultiple<T>(this IMapper mapper, params object[] sources) where T : class
        {
            T result = default;
            foreach (var source in sources)
            {
                if (result is default(T))
                {
                    result = mapper.Map<T>(source);
                    continue;
                }
                mapper.Map(source, result);
            }
            return result;
        }
    }
}
