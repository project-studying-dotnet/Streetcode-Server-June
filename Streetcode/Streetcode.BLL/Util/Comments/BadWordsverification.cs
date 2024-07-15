using mk.profanity;

namespace Streetcode.BLL.Util.Comments
{
    public class BadWordsVerification
    {
        public static bool NotContainBadWords(string text)
        {
            ProfanityFilter profanityFilter = new ProfanityFilter();

            return !profanityFilter.ContainsProfanity(text);
        }
    }
}
