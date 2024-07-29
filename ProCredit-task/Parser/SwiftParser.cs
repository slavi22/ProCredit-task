using System.Text;
using System.Text.RegularExpressions;
using NLog;
using ILogger = NLog.ILogger;

namespace ProCredit_task.Parser;

public class SwiftParser
{
    private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _message;

    public SwiftParser(IFormFile file)
    {
        _logger.Debug("SwiftParser Initialized");
        _message = ConvertFileContentToString(file);
    }

    private string ConvertFileContentToString(IFormFile file)
    {
        _logger.Debug("Converting the file contents to a string");
        var sb = new StringBuilder();
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            while (reader.Peek() >= 0)
            {
                sb.AppendLine(reader.ReadLine());
            }
        }
        _logger.Debug("Conversion finished. Returning the string.");
        return sb.ToString();
    }

    public Dictionary<string, string> ParseSwiftMessage()
    {
        _logger.Debug("Parsing the message contents into different blocks");
        var dict = new Dictionary<string, string>();
        var block1 = ExtractBlockBase(_message, 1); //basic header info
        var block2 = ExtractBlockBase(_message, 2); //application header info
        dict.Add("BasicHeaderInfo", block1);
        dict.Add("ApplicationHeaderInfo", block2);

        var block4Base = ExtractBlockBase(_message, 4);
        var block5Base = ExtractBlockBase(_message, 5);

        var block4 = ExtractLargerBlock(block4Base); //block4 - message texts; TransactionRef, RelatedRef, Narrative
        dict.Add("TransactionRef", block4["TransactionRef"]);
        dict.Add("RelatedRef", block4["RelatedRef"]);
        dict.Add("Narrative", block4["Narrative"]);
        var block5 = ExtractBlock5(block5Base); //block5 - trailer info; MAC, CHK
        dict.Add("MAC", block5["MAC"]);
        dict.Add("CHK", block5["CHK"]);

        if (dict.Count == 0)
        {
            _logger.Debug("Invalid content in the file.");
        }
        else
        {
            _logger.Debug("Blocks successfully parsed. Returning a dictionary with the relevant block information");
        }

        return dict;
    }


    private static string ExtractBlockBase(string message, int blockNumber)
    {
        var match = Regex.Match(message, $@"\{{{blockNumber}:(.*?(\{{.*?\}}.*?)*?)\}}", RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private static Dictionary<string, string> ExtractLargerBlock(string block)
    {
        var dict = new Dictionary<string, string>();
        var dictMatches = Regex.Matches(block, @":(\d{2}):(.+?)(?=:\d{2}:|$)", RegexOptions.Singleline);
        foreach (Match match in dictMatches)
        {
            var matchNumber = match.Groups[1].Value;
            var matchValue = match.Groups[2].Value.Replace("\r\n", " ").Trim();
            var matchValueCleanedUp = Regex.Replace(matchValue, @"\s+", " ");
            var cleanedUpValWithNewLines = Regex.Replace(matchValueCleanedUp, " . ", "\n");
            if (matchNumber == "20")
            {
                dict.Add("TransactionRef", cleanedUpValWithNewLines);
            }
            else if (matchNumber == "21")
            {
                dict.Add("RelatedRef", cleanedUpValWithNewLines);
            }
            else if (matchNumber == "79")
            {
                dict.Add("Narrative", cleanedUpValWithNewLines.TrimEnd(' ', '-'));
            }
        }

        return dict;
    }

    private static Dictionary<string, string> ExtractBlock5(string block)
    {
        var dict = new Dictionary<string, string>();
        var macExtracted = Regex.Match(block, @"\{MAC:(.*?)\}").Groups[1].Value;
        var chkExtracted = Regex.Match(block, @"\{CHK:(.*?)\}").Groups[1].Value;
        dict.Add("MAC", macExtracted);
        dict.Add("CHK", chkExtracted);
        return dict;
    }
}