using AIStudyAssistant.Application.DTOs;
using AIStudyAssistant.Application.Features.Subjects.Commands;
using AIStudyAssistant.Application.Features.Subjects.Queries;
using AIStudyAssistant.Application.Interfaces;
using AIStudyAssistant.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AIStudyAssistant.Application.Features.Summaries.Queries;
namespace AIStudyAssistant.API8.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SubjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IProgressCalculationService _progressService;

    public SubjectsController(
        IMediator mediator,
        IProgressCalculationService progressService)
    {
        _mediator = mediator;
        _progressService = progressService;
    }
    private int GetUserId()
    {
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"{claim.Type} = {claim.Value}");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID not found.");

        return int.Parse(userId);
    }

    // GET: api/Subjects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
    {
        Console.WriteLine("Authenticated: " + User.Identity?.IsAuthenticated);
        Console.WriteLine("Claims Count: " + User.Claims.Count());

        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"{claim.Type} = {claim.Value}");
        }
        var result = await _mediator.Send(new GetSubjectsQuery(GetUserId()));
        return Ok(result);
    }

    // GET: api/Subjects/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Subject>> GetSubject(int id)
    {
        var result = await _mediator.Send(
            new GetSubjectByIdQuery
            {
                SubjectId = id,
                UserId = GetUserId()
            });

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // POST
    [HttpPost]
    public async Task<ActionResult<Subject>> CreateSubject(CreateSubjectDto dto)
    {
        var result = await _mediator.Send(
            new CreateSubjectCommand
            {
                Dto = dto,
                UserId = GetUserId()
            });

        return CreatedAtAction(nameof(GetSubject),
            new { id = result.SubjectId }, result);
    }

    // PUT
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubject(int id, CreateSubjectDto dto)
    {
        await _mediator.Send(
            new UpdateSubjectCommand
            {
                SubjectId = id,
                UserId = GetUserId(),
                Dto = dto
            });

        return NoContent();
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        await _mediator.Send(
            new DeleteSubjectCommand
            {
                SubjectId = id,
                UserId = GetUserId()
            });

        return NoContent();
    }
  [HttpGet("download/{id}")]
  public async Task<IActionResult> Download(int id)
  {
    var summary = await _mediator.Send(
        new GetSummaryByIdQuery
        {
          SummaryId = id,
          UserId = GetUserId()
        });

    if (summary == null)
      return NotFound();

    if (string.IsNullOrWhiteSpace(summary.FilePath))
      return BadRequest("No file found.");

    if (!System.IO.File.Exists(summary.FilePath))
      return NotFound("File does not exist.");

    var bytes = await System.IO.File.ReadAllBytesAsync(summary.FilePath);

    return File(
        bytes,
        "application/pdf",
        summary.FileName
    );
  }
}
