
using System.Security.Claims;
using AIStudyAssistant.Application.Features.Quizzes.Commands;
using AIStudyAssistant.Application.Features.Quizzes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIStudyAssistant.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuizzesController : ControllerBase
{
  private readonly IMediator _mediator;

  public QuizzesController(IMediator mediator)
  {
    _mediator = mediator;
  }

  // =====================================================
  // GET ALL QUIZZES
  // =====================================================

  [HttpGet]
  public async Task<IActionResult> GetQuizzes()
  {
    var userId = GetUserId();

    var quizzes =
        await _mediator.Send(
            new GetQuizzesQuery(userId)
        );

    return Ok(quizzes);
  }

  // =====================================================
  // GET ONE QUIZ
  // =====================================================

  [HttpGet("{id}")]
  public async Task<IActionResult> GetQuiz(int id)
  {
    var quiz =
        await _mediator.Send(
            new GetQuizByIdQuery
            {
              QuizId = id,
              UserId = GetUserId()
            }
        );

    if (quiz == null)
    {
      return NotFound();
    }

    return Ok(quiz);
  }

  // =====================================================
  // CREATE QUIZ
  // =====================================================

  [HttpPost]
  public async Task<IActionResult> CreateQuiz(
      CreateQuizCommand command)
  {
    command.UserId = GetUserId();

    var result =
        await _mediator.Send(command);

    return Ok(result);
  }

  // =====================================================
  // UPDATE
  // =====================================================

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateQuiz(
      int id,
      UpdateQuizCommand command)
  {
    command.QuizId = id;
    command.UserId = GetUserId();

    await _mediator.Send(command);

    return NoContent();
  }

  // =====================================================
  // DELETE
  // =====================================================

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteQuiz(
      int id)
  {
    await _mediator.Send(
        new DeleteQuizCommand
        {
          QuizId = id,
          UserId = GetUserId()
        }
    );

    return NoContent();
  }

  // =====================================================
  // USER ID
  // =====================================================

  private int GetUserId()
  {
    var claim =
        User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

    if (string.IsNullOrWhiteSpace(claim))
    {
      throw new UnauthorizedAccessException(
          "User ID was not found in the authentication token."
      );
    }

    return int.Parse(claim);
  }
}

