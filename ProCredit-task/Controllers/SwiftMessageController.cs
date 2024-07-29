using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProCredit_task.Contracts;
using ProCredit_task.Extensions;
using ProCredit_task.Models;
using ProCredit_task.Parser;

namespace ProCredit_task.Controllers;

[ApiController]
[Route("api/v1/")]
public class SwiftMessageController : Controller
{
    private readonly ILogger<SwiftMessageController> _logger;
    private readonly IDatabase _db;

    public SwiftMessageController(ILogger<SwiftMessageController> logger, IDatabase db)
    {
        _logger = logger;
        _logger.LogDebug("NLog injected into SwiftMessageController");
        _db = db;
    }

    /// <summary>
    /// Parses the swift message and uploads it to the SQLite database
    /// </summary>
    /// <param name="swiftMessageFile"></param>
    /// <returns>Returns the newly created message</returns>
    /// <response code="201">Returns the newly created message</response>
    /// <response code="400">Responds with 400 if the file type is of unsupported type</response>
    [HttpPost]
    [Route("UploadMessage")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(MessageModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadSwiftMessage(
        [AllowedExtensions([".txt"], ErrorMessage = "Invalid file type. The only supported file type is '.txt'")] [Required]
        IFormFile swiftMessageFile)
    {
        try
        {
            var parser = new SwiftParser(swiftMessageFile);
            var message = parser.ParseSwiftMessage();
            var result = await _db.UploadMessageToDatabase(message);
            _logger.LogDebug($"Successfully added a message with id - \"{result.Id}\" to the database!");
            return StatusCode(201, result);
        }
        catch (KeyNotFoundException)
        {
            //https://stackoverflow.com/a/70922358
            ModelState.AddModelError("swiftMessageFile", "Invalid content in the file.");
            //https://stackoverflow.com/a/56126119
            return ValidationProblem(ModelState);
        }

    }
}