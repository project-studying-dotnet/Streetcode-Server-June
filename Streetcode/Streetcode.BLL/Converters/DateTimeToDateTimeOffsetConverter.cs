using AutoMapper;

namespace Streetcode.BLL.Converters
{
    public class DateTimeToDateTimeOffsetConverter : IValueConverter<DateTime, DateTimeOffset>
    {
        public DateTimeOffset Convert(DateTime sourceMember, ResolutionContext context)
        {
            return (DateTimeOffset)DateTime.SpecifyKind(sourceMember, DateTimeKind.Utc);           
        }
    }
}
