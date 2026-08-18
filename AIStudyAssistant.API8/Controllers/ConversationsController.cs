using System.Security.Claims;
using AIStudyAssistant.Application.Features.Conversations.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIStudyAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversationsController : ControllerBase
{
  private readonly IMediator _mediator;

  public ConversationsController(IMediator mediator)
  {
    _mediator = mediator;
  }


  private int GetUserId()
  {
    return int.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!
    );
  }


  // Get previous chats for sidebar
  [HttpGet("history")]
  public async Task<IActionResult> GetHistory()
  {
    var result = await _mediator.Send(
        new GetConversationsQuery
        {
          UserId = GetUserId()
        });

    return Ok(result);
  }
}
