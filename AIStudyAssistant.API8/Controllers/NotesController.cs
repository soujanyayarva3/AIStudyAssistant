using AIStudyAssistant.Application.Features.Notes.Commands;
using AIStudyAssistant.Application.Features.Notes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIStudyAssistant.API8.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
  private readonly IMediator _mediator;

  public NotesController(IMediator mediator)
  {
    _mediator = mediator;
  }

  // =====================================================
  // GET ALL NOTES
  // GET: api/Notes
  // =====================================================

  [HttpGet]
  public async Task<IActionResult> GetNotes()
  {
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (!int.TryParse(userIdClaim, out int userId))
    {
      return Unauthorized();
    }

    var notes =
        await _mediator.Send(
            new GetNotesQuery(userId)
        );

    return Ok(notes);
  }

  // =====================================================
  // CREATE NOTE
  // POST: api/Notes
  // =====================================================

  [HttpPost]
  public async Task<IActionResult> CreateNote(
      [FromBody] CreateNoteDto dto)
  {
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (!int.TryParse(userIdClaim, out int userId))
    {
      return Unauthorized();
    }

    if (dto.SubjectId <= 0)
    {
      return BadRequest(
          "A valid SubjectId is required."
      );
    }

    if (string.IsNullOrWhiteSpace(dto.Title))
    {
      return BadRequest(
          "Note title is required."
      );
    }

    if (string.IsNullOrWhiteSpace(dto.Content))
    {
      return BadRequest(
          "Note content is required."
      );
    }

    var command = new CreateNoteCommand
    {
      Title = dto.Title,
      Content = dto.Content,
      SubjectId = dto.SubjectId,
      UserId = userId
    };

    var result =
        await _mediator.Send(command);

    return Ok(new
    {
      message = "Note created successfully.",
      data = result
    });
  }

  // =====================================================
  // UPDATE NOTE
  // PUT: api/Notes/{id}
  // =====================================================

  [HttpPut("{id:int}")]
  public async Task<IActionResult> UpdateNote(
      int id,
      [FromBody] UpdateNoteDto dto)
  {
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (!int.TryParse(userIdClaim, out int userId))
    {
      return Unauthorized();
    }

    if (id <= 0)
    {
      return BadRequest(
          "A valid NoteId is required."
      );
    }

    if (dto.SubjectId <= 0)
    {
      return BadRequest(
          "A valid SubjectId is required."
      );
    }

    if (string.IsNullOrWhiteSpace(dto.Title))
    {
      return BadRequest(
          "Note title is required."
      );
    }

    if (string.IsNullOrWhiteSpace(dto.Content))
    {
      return BadRequest(
          "Note content is required."
      );
    }

    var command = new UpdateNoteCommand
    {
      NoteId = id,
      Title = dto.Title,
      Content = dto.Content,
      SubjectId = dto.SubjectId,
      UserId = userId
    };

    await _mediator.Send(command);

    return Ok(new
    {
      message = "Note updated successfully."
    });
  }

  // =====================================================
  // DELETE NOTE
  // DELETE: api/Notes/{id}
  // =====================================================

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteNote(
      int id)
  {
    var userIdClaim =
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (!int.TryParse(userIdClaim, out int userId))
    {
      return Unauthorized();
    }

    if (id <= 0)
    {
      return BadRequest(
          "A valid NoteId is required."
      );
    }

    Console.WriteLine(
        "=========================================="
    );

    Console.WriteLine("DELETE NOTE");
    Console.WriteLine($"NoteId: {id}");
    Console.WriteLine($"UserId: {userId}");

    Console.WriteLine(
        "=========================================="
    );

    await _mediator.Send(
        new DeleteNoteCommand
        {
          NoteId = id,
          UserId = userId
        }
    );

    return NoContent();
  }

  // =====================================================
  // CREATE NOTE DTO
  // =====================================================

  public class CreateNoteDto
  {
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int SubjectId { get; set; }
  }

  // =====================================================
  // UPDATE NOTE DTO
  // =====================================================

  public class UpdateNoteDto
  {
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int SubjectId { get; set; }
  }
}
