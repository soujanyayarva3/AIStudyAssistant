using System.Security.Claims;
using AIStudyAssistant.API.DTOs;
using AIStudyAssistant.Application.Features.StudyPlans.Commands;
using AIStudyAssistant.Application.Features.StudyPlans.Queries;
using AIStudyAssistant.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIStudyAssistant.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StudyPlansController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly IProgressCalculationService _progressService;

  public StudyPlansController(
      IMediator mediator,
      IProgressCalculationService progressService)
  {
    _mediator = mediator;
    _progressService = progressService;
  }

  // =====================================================
  // GET USER ID
  // =====================================================

  private int GetUserId()
  {
    var userId =
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
      throw new Exception("UserId claim missing in token");

    return int.Parse(userId);
  }

  // =====================================================
  // GET ALL STUDY PLANS
  // =====================================================

  [HttpGet]
  public async Task<ActionResult<IEnumerable<object>>> GetStudyPlans()
  {
    var plans = await _mediator.Send(
        new GetStudyPlansQuery(GetUserId())
    );

    return Ok(plans);
  }

  // =====================================================
  // GET STUDY PLAN BY ID
  // =====================================================

  [HttpGet("{id}")]
  public async Task<ActionResult> GetStudyPlan(int id)
  {
    var plan = await _mediator.Send(
        new GetStudyPlanByIdQuery
        {
          PlanId = id,
          UserId = GetUserId()
        }
    );

    if (plan == null)
      return NotFound();

    return Ok(plan);
  }

  // =====================================================
  // CREATE STUDY PLAN
  // =====================================================

  [HttpPost]
  public async Task<ActionResult> CreateStudyPlan(
      [FromBody] CreateStudyPlanDto dto)
  {
    var plan = await _mediator.Send(
        new CreateStudyPlanCommand
        {
          TaskName = dto.Title,

          Description = dto.Description,

          StartDate = DateTime.UtcNow,

          // FIXED: TargetDate -> DueDate
          DueDate = DateTime.SpecifyKind(
                dto.DueDate,
                DateTimeKind.Utc
            ),

          Status = dto.Status,

          UserId = GetUserId()
        }
    );

    await _progressService.UpdateProgressAsync(
        GetUserId()
    );

    return CreatedAtAction(
        nameof(GetStudyPlan),
        new { id = plan.PlanId },
        plan
    );
  }

  // =====================================================
  // UPDATE STUDY PLAN
  // =====================================================

  [HttpPut("{id}")]
  public async Task<ActionResult> UpdateStudyPlan(
      int id,
      [FromBody] CreateStudyPlanDto dto)
  {
    await _mediator.Send(
        new UpdateStudyPlanCommand
        {
          PlanId = id,

          TaskName = dto.Title,

          Description = dto.Description,

          StartDate = DateTime.UtcNow,

          // FIXED: TargetDate -> DueDate
          DueDate = DateTime.SpecifyKind(
                dto.DueDate,
                DateTimeKind.Utc
            ),

          Status = dto.Status,

          UserId = GetUserId()
        }
    );

    await _progressService.UpdateProgressAsync(
        GetUserId()
    );

    return NoContent();
  }

  // =====================================================
  // DELETE STUDY PLAN
  // =====================================================

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteStudyPlan(int id)
  {
    await _mediator.Send(
        new DeleteStudyPlanCommand
        {
          PlanId = id,
          UserId = GetUserId()
        }
    );

    await _progressService.UpdateProgressAsync(
        GetUserId()
    );

    return NoContent();
  }

  // =====================================================
  // UPDATE STATUS
  // =====================================================

  [HttpPatch("{id}/status")]
  public async Task<ActionResult> UpdateStatus(
      int id,
      [FromBody] UpdateStudyPlanStatusDto dto)
  {
    var plan = await _mediator.Send(
        new GetStudyPlanByIdQuery
        {
          PlanId = id,
          UserId = GetUserId()
        }
    );

    if (plan == null)
      return NotFound();

    await _mediator.Send(
        new UpdateStudyPlanCommand
        {
          PlanId = id,

          TaskName = plan.TaskName,

          // Use the description already stored in the plan
          Description = plan.Description,

          StartDate = plan.StartDate,

          DueDate = plan.DueDate,

          Status = dto.Status,

          UserId = GetUserId()
        }
    );

    await _progressService.UpdateProgressAsync(
        GetUserId()
    );

    return Ok();
  }
}
