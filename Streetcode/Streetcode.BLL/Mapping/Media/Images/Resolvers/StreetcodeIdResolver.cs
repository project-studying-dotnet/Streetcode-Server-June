using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.Mapping.Media.Images.Resolvers;

public class StreetcodeIdResolver : IValueResolver<Art, StreetcodeFilterResultDTO, int>
{
    public int Resolve(Art source, StreetcodeFilterResultDTO destination, int destMember, ResolutionContext context)
    {
        return source.StreetcodeArts.Find(sa => sa.Streetcode != null) !.StreetcodeId;
    }
}