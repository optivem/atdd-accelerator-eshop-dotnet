using Microsoft.AspNetCore.Mvc;

namespace Optivem.AtddAccelerator.Template.Monolith.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public TodosController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTodo(int id)
    {
        var baseUrl = _configuration["ExternalApis:JsonPlaceholder"] ?? "https://jsonplaceholder.typicode.com";
        
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync($"{baseUrl}/todos/{id}");
        
        if (response.IsSuccessStatusCode)
        {
            var jsonContent = await response.Content.ReadAsStringAsync();
            return Content(jsonContent, "application/json");
        }
        
        return NotFound();
    }
}
