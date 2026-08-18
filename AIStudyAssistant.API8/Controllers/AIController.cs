using AIStudyAssistant.API.DTOs;
using AIStudyAssistant.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIStudyAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : ControllerBase
{
  private readonly OllamaService _ollamaService;

  public AIController(OllamaService ollamaService)
  {
    _ollamaService = ollamaService;
  }

  [HttpPost("ask")]
  public async Task<ActionResult<AIResponseDto>> Ask(AskAIDto request)
  {
    if (string.IsNullOrWhiteSpace(request.Question))
    {
      return BadRequest("Question cannot be empty.");
    }

    var answer = await _ollamaService.GenerateResponseAsync(request.Question);

    return Ok(new AIResponseDto
    {
      Response = answer
    });
  }
}
