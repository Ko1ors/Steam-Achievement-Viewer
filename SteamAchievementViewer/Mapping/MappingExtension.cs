using AutoMapper;
using System.ComponentModel;

namespace SteamAchievementViewer.Mapping
{
    public static class MappingExtension
    {
        public static T MapMultiple<T>(this IMapper mapper, object firstSource, object secondSource)
        {
            return mapper.Map(secondSource, mapper.Map<T>(firstSource));
        }
    }
}
