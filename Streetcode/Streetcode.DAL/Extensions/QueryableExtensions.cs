using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Streetcode.BLL.Specification.Streetcode.Streetcode.NewFolder;

public static class SpecificationExtensions
{
    public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, ISpecification<T> specification) 
        where T : class
    {
        return SpecificationEvaluator.Default.GetQuery(
            query: query.AsNoTracking(),
            specification: specification);
    }
}