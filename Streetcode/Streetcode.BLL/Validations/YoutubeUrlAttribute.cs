using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.Validations;

public class YoutubeUrlAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var url = value as string;

        if (string.IsNullOrEmpty(url))
        {
            return ValidationResult.Success;
        }

        if (Uri.IsWellFormedUriString(url, UriKind.Absolute) && url.StartsWith("https://www.youtube.com/"))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("The video URL is not a valid URL - it should start with 'https://www.youtube.com/'");
    }
}