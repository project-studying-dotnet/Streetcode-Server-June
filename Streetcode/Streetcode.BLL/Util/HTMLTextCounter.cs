using HtmlAgilityPack;

namespace Streetcode.BLL.Util
{
    public static class HTMLTextCounter
    {
        public static int OnlyTextCount(this string text)
        {
            if (text == null || text == string.Empty)
            {
                return 0;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);

            string taglessText = doc.DocumentNode.InnerText;

            return taglessText.Length;
        }
    }
}
