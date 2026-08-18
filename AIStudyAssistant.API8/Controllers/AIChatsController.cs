using System.Security.Claims;
using AIStudyAssistant.Application.Features.AIChats.Commands;
using AIStudyAssistant.Application.Features.AIChats.Queries;
using AIStudyAssistant.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIStudyAssistant.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AIChatsController : ControllerBase
{
  private readonly IMediator _mediator;

  public AIChatsController(IMediator mediator)
  {
    _mediator = mediator;
  }


  private int GetUserId()
  {
    return int.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!
    );
  }


  // Get all chats of logged-in user
  [HttpGet]
  public async Task<ActionResult<IEnumerable<AIChat>>> GetChats()
  {
    var chats = await _mediator.Send(
        new GetAIChatsQuery(GetUserId())
    );

    return Ok(chats);
  }


  // Get single chat
  [HttpGet("{id}")]
  public async Task<ActionResult<AIChat>> GetChat(int id)
  {
    var chat = await _mediator.Send(
        new GetAIChatByIdQuery
        {
          ChatId = id,
          UserId = GetUserId()
        }
    );

    if (chat == null)
      return NotFound();

    return Ok(chat);
  }


  // Get messages inside one conversation
  [HttpGet("conversation/{conversationId}")]
  public async Task<IActionResult> GetChatsByConversation(
      int conversationId)
  {
    var result = await _mediator.Send(
        new GetChatsByConversationQuery(conversationId)
    );

    return Ok(result);
  }


  // Create new AI chat message
  [HttpPost]
  public async Task<ActionResult<AIChat>> CreateChat(
      CreateAIChatCommand command)
  {
    command.UserId = GetUserId();

    var chat = await _mediator.Send(command);

    return Ok(chat);
  }


  // Update chat
  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateChat(
      int id,
      UpdateAIChatCommand command)
  {
    command.ChatId = id;
    command.UserId = GetUserId();

    await _mediator.Send(command);

    return NoContent();
  }


  // Delete chat
  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteChat(int id)
  {
    await _mediator.Send(
        new DeleteAIChatCommand
        {
          ChatId = id,
          UserId = GetUserId()
        }
    );

    return NoContent();
  }
}
