using System.ComponentModel;

namespace Streetcode.DAL.Enums;

public enum SourceType
{
    [Description("wow-facts")]
    WowFacts,
    [Description("text")]
    Text,
    [Description("art-gallery")]
    ArtGallery,
    [Description("timeline")]
    Timeline
}
