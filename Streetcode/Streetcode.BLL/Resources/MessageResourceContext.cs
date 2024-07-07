using System.Resources;

namespace Streetcode.BLL.Resources
{
    public class MessageResourceContext
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager("Streetcode.BLL.Resources.ErrorMessages", typeof(ErrorMessages).Assembly);

        public static string GetMessage(string error, params object[] formatValue)
        {
            string entityId = string.Empty;
            string requestType = formatValue[0].GetType().ToString();
            var entityName = EntityFilter(requestType);

            if(formatValue.Length > 0)
            {
                entityId = formatValue[0].ToString();
            }

            if (entityName != null)
            {
                try
                {
                   return string.Format(error, entityName, entityId);
                }
                catch
                {
                    return "An unknown error occurred.";
                }
            }

            return "An unknown error occurred.";
        }
        
        public static string GetMessage(string error)
        {
            string? message = _resourceManager.GetString(error);

            if (message == null)
            {
                return $"Error message '{error}' not found.";
            }

            return message;
        }
        
        private static string EntityFilter(string input)
        {
            var parts = input.Split('.');

            if (parts[^3] == "StreetcodeArt") 
            {
                return "Art";
            }

            if (parts[^3] == "SourceLinkCategory")
            {
                return "Categories";
            }

            return parts[^3];
        }
    }
}