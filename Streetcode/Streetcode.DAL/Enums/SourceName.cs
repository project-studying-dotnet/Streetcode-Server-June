using System.ComponentModel;

namespace Streetcode.DAL.Enums;

public enum SourceName
{
    [Description("Wow-факти")]
    WowFacts,
    [Description("Текст")]
    Text,
    [Description("Арт-галерея")]
    ArtGallery,
    [Description("Хронологія")]
    Timeline
}
