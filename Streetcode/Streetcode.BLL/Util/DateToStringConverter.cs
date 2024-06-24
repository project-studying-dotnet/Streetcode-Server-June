using Streetcode.DAL.Enums;

namespace Streetcode.BLL.Util
{
    public class DateToStringConverter
    {
        public static string FromDateToString(DateTime date, DateViewPattern pattern)
        {
            return pattern switch
            {
                DateViewPattern.Year => date.ToString("yyyy"),
                DateViewPattern.MonthYear => date.ToString("yyyy, MMMM"),
                DateViewPattern.SeasonYear => $"{GetSeason(date)} {date.Year}",
                DateViewPattern.DateMonthYear => date.ToString("yyyy, d MMMM"),
                _ =>""
            };
        }

        public static string CreateDateString(DateTime start, DateTime? end)
        {
            var startStr = $"{start.Day} {GetMonthNounInGenitiveCase(start)} {start.Year}";
            var endStr = end is DateTime e ? $"{e.Day} {GetMonthNounInGenitiveCase(e)} {e.Year}" : null;

            return endStr is not null ? $"{startStr} - {endStr}" : startStr;
        }

        private static string GetMonthNounInGenitiveCase(DateTime date) => date.Month switch
        {
            1 => "січня",
            2 => "лютого",
            3 => "березня",
            4 => "квітня",
            5 => "травня",
            6 => "червня",
            7 => "липня",
            8 => "серпня",
            9 => "вересня",
            10 => "жовтня",
            11 => "листопада",
            12 => "грудня",
            _ => throw new InvalidOperationException("No such month"),
        };

        private static string GetSeason(DateTime dateTime)
        {
            if (dateTime.Month < 3 || dateTime.Month == 12)
            {
                return "зима";
            }
            else if (dateTime.Month >= 3 && dateTime.Month < 6)
            {
                return "весна";
            }
            else if(dateTime.Month >= 6 && dateTime.Month < 9)
            {
                return "літо";
            }
            else
            {
                return "осінь";
            }
        }
    }
}
