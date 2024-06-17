using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Streetcode.DAL.Entities.Streetcode.TextContent;

public class Term
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public List<RelatedTerm> RelatedTerms { get; set; } = new();
}