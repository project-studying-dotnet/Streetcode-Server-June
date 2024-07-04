using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.Mapping.Media.Images.Resolvers;
public class StreetcodeIndexResolver : IValueResolver<Art, StreetcodeFilterResultDTO, int>
{
    public int Resolve(Art source, StreetcodeFilterResultDTO destination, int destMember, ResolutionContext context)
    {
        var streetcodeArt = source.StreetcodeArts.FirstOrDefault();
        return streetcodeArt?.Streetcode != null ? streetcodeArt.Streetcode.Index : 0;
    }
}