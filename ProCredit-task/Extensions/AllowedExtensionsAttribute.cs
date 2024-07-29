using System.ComponentModel.DataAnnotations;

namespace ProCredit_task.Extensions;

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;

    public AllowedExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions;
    }


    public override bool IsValid(object value)
    {
        if (value is null)
            return true;

        var file = value as IFormFile;
        var extension = Path.GetExtension(file.FileName);
        if (!_extensions.Contains(extension.ToLower()))
            return false;

        return true;
    }
}