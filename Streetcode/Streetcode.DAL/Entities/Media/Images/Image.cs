﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Entities.Team;

namespace Streetcode.DAL.Entities.Media.Images;

public class Image
{
    public int Id { get; set; }

    public string? Base64 { get; set; } = string.Empty;

    public string? BlobName { get; set; } = string.Empty;

    public string? MimeType { get; set; } = string.Empty;

    public ImageDetails? ImageDetails { get; set; } = new();

    public List<StreetcodeContent> Streetcodes { get; set; } = new ();

    public List<Fact> Facts { get; set; } = new ();

    public Art? Art { get; set; }

    public Partner? Partner { get; set; } = new();

    public List<SourceLinkCategory> SourceLinkCategories { get; set; } = new ();

    public News.News? News { get; set; }
    public TeamMember? TeamMember { get; set; } = new();
}
