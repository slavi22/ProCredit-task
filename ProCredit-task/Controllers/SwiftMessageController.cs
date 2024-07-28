using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProCredit_task.Contracts;
using ProCredit_task.Parser;

namespace ProCredit_task.Controllers;

[Route("api/v1/")]
[ApiController]
public class SwiftMessageController : Controller
{
    private readonly IDatabase _db;

    public SwiftMessageController(IDatabase db)
    {
        _db = db;
    }


    [HttpPost]
    [Route("UploadMessage")]
    public async Task<IActionResult> UploadSwiftMessage(IFormFile swiftMessage)
    {
        var parser = new SwiftParser(swiftMessage);
        var dict = parser.ParseSwiftMessage();
        foreach (var item in dict)
        {
            Console.WriteLine($"{item.Key} - {item.Value}");
        }
        //await _db.GetAllMessages();
        return Ok();
    }
}