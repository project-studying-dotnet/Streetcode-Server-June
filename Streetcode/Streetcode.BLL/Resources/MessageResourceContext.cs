using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.DAL.Entities.Sources;

namespace Streetcode.BLL.Resources
{
    public class MessageResourceContext
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager("Streetcode.BLL.Resources.ErrorMessages", typeof(ErrorMessages).Assembly);
        private static string? _entityId;
        private static string? _entityName;
        public static string GetMessage(string error, params object[] formatValue)
        {
            string requestType = formatValue[0].GetType().ToString();
            _entityName = EntityFilter(requestType);

            if(formatValue.Length > 1)
            {
                _entityId = formatValue[1].ToString();
            }

            if (_entityName != null)
            {
                try
                {
                   return string.Format(error, _entityName, _entityId);
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
        
        public static string GetMessage(string error, object? formatValue)
        {
            string? message = _resourceManager.GetString(error);

            if (message == null)
            {
                return $"Error message '{error}' not found.";
            }

            if (formatValue != null)
            {
                message = string.Format(message, formatValue);
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