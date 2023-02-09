using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace SteamAchievementViewer.Mapping
{
    public class GenericSingleToListConverter<T1, T2> : ITypeConverter<T1, List<T2>>
    {
        public List<T2> Convert(T1 source, List<T2> destination, ResolutionContext context)
        {
            return destination.Select(i => context.Mapper.Map(source, i)).ToList();
        }

    }
}
