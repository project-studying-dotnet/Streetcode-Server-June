using AutoMapper;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.BLL.Mapping.Timeline;

public class TimelineItemProfile : Profile
{
    public TimelineItemProfile()
    {
        CreateMap<TimelineItem, TimelineItemDTO>()
            .ForMember(dest => dest.HistoricalContexts, opt => opt.MapFrom(x => x.HistoricalContextTimelines
                .Select(hct => new HistoricalContextDTO
                {
                    Id = hct.HistoricalContextId,
                    Title = hct.HistoricalContext!.Title
                }).ToList()))
            .ReverseMap();

        CreateMap<TimelineItem, CreateTimelineItemDTO>()
            .ForMember(dest => dest.HistoricalContexts, opt => opt.MapFrom(x => x.HistoricalContextTimelines
                .Select(hct => new HistoricalContextDTO
                {
                    Id = hct.HistoricalContextId,
                    Title = hct.HistoricalContext!.Title
                }).ToList()))
            .ReverseMap();
        CreateMap<TimelineItem, StreetcodeFilterResultDTO>()
           .ForMember(dest => dest.StreetcodeId, opt => opt.MapFrom(src => src.StreetcodeId))
           .ForMember(dest => dest.StreetcodeIndex, opt => opt.MapFrom(src => src.Streetcode!.Index))
           .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Title))
           .ForMember(dest => dest.BlockName, opt => opt.MapFrom("timeline"))
           .ForMember(dest => dest.SourceName, opt => opt.MapFrom("Хронологія"));
    }
}