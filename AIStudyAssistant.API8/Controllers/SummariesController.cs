using AIStudyAssistant.Application.Features.Summaries.Commands;
using AIStudyAssistant.Application.Features.Summaries.Queries;
using AIStudyAssistant.Application.Interfaces;
using AIStudyAssistant.Application.Services;
using AIStudyAssistant.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;

namespace AIStudyAssistant.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SummariesController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly IProgressCalculationService _progressService;
  private readonly OCRService _ocrService;
  private readonly OllamaService _ollamaService;

  public SummariesController(
      IMediator mediator,
      IProgressCalculationService progressService,
      OCRService ocrService,
      OllamaService ollamaService)
  {
    _mediator = mediator;
    _progressService = progressService;
    _ocrService = ocrService;
    _ollamaService = ollamaService;
  }

  // =========================================================
  // GET USER ID
  // =========================================================

  private int GetUserId()
  {
    var userId =
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrWhiteSpace(userId))
    {
      throw new UnauthorizedAccessException(
          "User ID not found in authentication token."
      );
    }

    if (!int.TryParse(userId, out var parsedUserId))
    {
      throw new UnauthorizedAccessException(
          "Invalid User ID in authentication token."
      );
    }

    return parsedUserId;
  }

  // =========================================================
  // GET ALL SUMMARIES
  // GET: api/Summaries
  // =========================================================

  [HttpGet]
  public async Task<IActionResult> GetSummaries()
  {
    var userId = GetUserId();

    Console.WriteLine(
        $"GET SUMMARIES - UserId: {userId}"
    );

    var summaries =
        await _mediator.Send(
            new GetSummariesQuery(userId)
        );

    return Ok(summaries);
  }

  // =========================================================
  // GET ONE SUMMARY
  // GET: api/Summaries/62
  // =========================================================

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetSummary(int id)
  {
    var userId = GetUserId();

    Console.WriteLine(
        $"GET SUMMARY - SummaryId: {id}, UserId: {userId}"
    );

    var summary =
        await _mediator.Send(
            new GetSummaryByIdQuery
            {
              SummaryId = id,
              UserId = userId
            }
        );

    if (summary == null)
    {
      Console.WriteLine(
          $"SUMMARY NOT FOUND - Id: {id}, UserId: {userId}"
      );

      return NotFound(
          new
          {
            message =
                  $"Summary with ID {id} was not found for the current user."
          }
      );
    }

    return Ok(summary);
  }

  // =========================================================
  // CREATE TEXT SUMMARY
  // POST: api/Summaries
  // =========================================================

  [HttpPost]
  public async Task<IActionResult> Create(
      [FromBody] CreateSummaryCommand command)
  {
    var userId = GetUserId();

    command.UserId = userId;

    Console.WriteLine(
        $"CREATE SUMMARY - UserId: {userId}"
    );

    var result =
        await _mediator.Send(command);

    await _progressService.UpdateProgressAsync(
        userId
    );

    return CreatedAtAction(
        nameof(GetSummary),
        new
        {
          id = result.SummaryId
        },
        result
    );
  }

  // =========================================================
  // IMAGE SUMMARY
  // POST: api/Summaries/image
  // =========================================================

  [HttpPost("image")]
  public async Task<IActionResult> SummarizeImage(
      IFormFile image,
      [FromForm] string title,
      [FromForm] string summaryStyle)
  {
    var userId = GetUserId();

    if (image == null || image.Length == 0)
    {
      return BadRequest(
          new
          {
            message = "No image uploaded."
          }
      );
    }

    var uploadsFolder =
        Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            "Summaries"
        );

    Directory.CreateDirectory(
        uploadsFolder
    );

    var savedFileName =
        $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

    var filePath =
        Path.Combine(
            uploadsFolder,
            savedFileName
        );

    await using (
        var stream =
            new FileStream(
                filePath,
                FileMode.Create
            ))
    {
      await image.CopyToAsync(stream);
    }

    var extractedText =
        _ocrService.ExtractText(
            filePath
        );

    if (string.IsNullOrWhiteSpace(extractedText))
    {
      return BadRequest(
          new
          {
            message = "No readable text found in image."
          }
      );
    }

    var command =
        new CreateSummaryCommand
        {
          Title = title,
          SummaryStyle = summaryStyle,
          OriginalText = extractedText,
          SummaryText = "",
          UserId = userId
        };

    var result =
        await _mediator.Send(command);

    await _progressService.UpdateProgressAsync(
        userId
    );

    return Ok(result);
  }

  // =========================================================
  // PDF SUMMARY
  // POST: api/Summaries/pdf
  // =========================================================

  [HttpPost("pdf")]
  public async Task<IActionResult> SummarizePdf(
      IFormFile pdf,
      [FromForm] string title,
      [FromForm] string summaryStyle)
  {
    var userId = GetUserId();

    if (pdf == null || pdf.Length == 0)
    {
      return BadRequest(
          new
          {
            message = "No PDF uploaded."
          }
      );
    }

    string extractedText;

    await using (
        var stream =
            pdf.OpenReadStream())
    using (
        var document =
            PdfDocument.Open(stream))
    {
      var builder =
          new StringBuilder();

      foreach (
          var page
          in document.GetPages())
      {
        builder.AppendLine(
            page.Text
        );
      }

      extractedText =
          builder.ToString();
    }

    if (string.IsNullOrWhiteSpace(extractedText))
    {
      return BadRequest(
          new
          {
            message = "No readable text found in PDF."
          }
      );
    }

    var command =
        new CreateSummaryCommand
        {
          Title = title,
          SummaryStyle = summaryStyle,
          OriginalText = extractedText,
          SummaryText = "",
          UserId = userId
        };

    var result =
        await _mediator.Send(command);

    await _progressService.UpdateProgressAsync(
        userId
    );

    return Ok(result);
  }

  // =========================================================
  // DOWNLOAD SUMMARY AS PDF
  // GET: api/Summaries/download/62
  // =========================================================

  [HttpGet("download/{id:int}")]
  public async Task<IActionResult> DownloadSummaryPdf(
      int id)
  {
    var userId = GetUserId();

    Console.WriteLine(
        "=========================================="
    );

    Console.WriteLine(
        $"DOWNLOAD SUMMARY"
    );

    Console.WriteLine(
        $"SummaryId: {id}"
    );

    Console.WriteLine(
        $"UserId: {userId}"
    );

    Console.WriteLine(
        "=========================================="
    );

    var summary =
        await _mediator.Send(
            new GetSummaryByIdQuery
            {
              SummaryId = id,
              UserId = userId
            }
        );

    if (summary == null)
    {
      Console.WriteLine(
          $"DOWNLOAD FAILED - Summary {id} not found for User {userId}"
      );

      return NotFound(
          new
          {
            message =
                  $"Summary with ID {id} was not found."
          }
      );
    }

    Console.WriteLine(
        $"SUMMARY FOUND: {summary.Title}"
    );

    QuestPDF.Settings.License =
        LicenseType.Community;

    // =====================================================
    // KEYWORDS
    // =====================================================

    var keywords =
        new List<string>();

    if (!string.IsNullOrWhiteSpace(
        summary.Keywords))
    {
      try
      {
        keywords =
            JsonSerializer.Deserialize<List<string>>(
                summary.Keywords
            )
            ?? new List<string>();
      }
      catch
      {
        keywords =
            new List<string>();
      }
    }

    // =====================================================
    // QUESTIONS
    // =====================================================

    var questions =
        new List<string>();

    if (!string.IsNullOrWhiteSpace(
        summary.Questions))
    {
      try
      {
        questions =
            JsonSerializer.Deserialize<List<string>>(
                summary.Questions
            )
            ?? new List<string>();
      }
      catch
      {
        questions =
            new List<string>();
      }
    }

    // =====================================================
    // GENERATE PDF
    // =====================================================

    var pdfBytes =
        Document.Create(container =>
        {
          container.Page(page =>
          {
            page.Size(PageSizes.A4);

            page.Margin(40);

            // HEADER
            page.Header()
                    .Column(header =>
                    {
                      header.Item()
                              .Text(
                                  "AI Study Assistant"
                              )
                              .FontSize(24)
                              .Bold();

                      header.Item()
                              .Text(
                                  "Study Summary Report"
                              )
                              .FontSize(16);

                      header.Item()
                              .PaddingTop(5)
                              .LineHorizontal(1);
                    });

            // CONTENT
            page.Content()
                    .Column(col =>
                    {
                      col.Spacing(15);

                      col.Item()
                              .Text(
                                  summary.Title ??
                                  "AI Study Summary"
                              )
                              .FontSize(22)
                              .Bold();

                      if (!string.IsNullOrWhiteSpace(
                              summary.SummaryStyle))
                      {
                        col.Item()
                                .Text(
                                    $"Summary Style: {summary.SummaryStyle}"
                                )
                                .FontSize(11);
                      }

                      col.Item()
                              .Text(
                                  $"Generated: {summary.CreatedDate:dd MMM yyyy}"
                              )
                              .FontSize(10);

                      // SUMMARY
                      col.Item()
                              .PaddingTop(10)
                              .Text("Summary")
                              .FontSize(18)
                              .Bold();

                      col.Item()
                              .Text(
                                  string.IsNullOrWhiteSpace(
                                      summary.SummaryText)
                                      ? "No summary available."
                                      : summary.SummaryText
                              )
                              .FontSize(12)
                              .LineHeight(1.4f);

                      // KEYWORDS
                      col.Item()
                              .PaddingTop(10)
                              .Text("Key Concepts")
                              .FontSize(18)
                              .Bold();

                      if (keywords.Count > 0)
                      {
                        foreach (
                                var keyword
                                in keywords)
                        {
                          col.Item()
                                  .Text(
                                      $"• {keyword}"
                                  )
                                  .FontSize(12);
                        }
                      }
                      else
                      {
                        col.Item()
                                .Text(
                                    "No keywords available."
                                );
                      }

                      // QUESTIONS
                      col.Item()
                              .PaddingTop(10)
                              .Text("Viva Questions")
                              .FontSize(18)
                              .Bold();

                      if (questions.Count > 0)
                      {
                        for (
                                var i = 0;
                                i < questions.Count;
                                i++)
                        {
                          col.Item()
                                  .Text(
                                      $"{i + 1}. {questions[i]}"
                                  )
                                  .FontSize(12);
                        }
                      }
                      else
                      {
                        col.Item()
                                .Text(
                                    "No questions available."
                                );
                      }
                    });

            // FOOTER
            page.Footer()
                    .AlignCenter()
                    .Text(
                        "Generated by AI Study Assistant"
                    )
                    .FontSize(9);
          });
        })
        .GeneratePdf();

    Console.WriteLine(
        $"PDF GENERATED SUCCESSFULLY: {pdfBytes.Length} bytes"
    );

    // =====================================================
    // SAFE FILE NAME
    // =====================================================

    var safeTitle =
        string.IsNullOrWhiteSpace(
            summary.Title)
            ? "AI-Study-Summary"
            : summary.Title;

    foreach (
        var invalidChar
        in Path.GetInvalidFileNameChars())
    {
      safeTitle =
          safeTitle.Replace(
              invalidChar,
              '_'
          );
    }

    var fileName =
        $"{safeTitle}.pdf";

    return File(
        pdfBytes,
        "application/pdf",
        fileName
    );
  }

  // =========================================================
  // UPDATE SUMMARY
  // PUT: api/Summaries/62
  // =========================================================

  [HttpPut("{id:int}")]
  public async Task<IActionResult> Update(
      int id,
      [FromBody] UpdateSummaryCommand command)
  {
    var userId = GetUserId();

    command.SummaryId = id;
    command.UserId = userId;

    await _mediator.Send(command);

    await _progressService.UpdateProgressAsync(
        userId
    );

    return NoContent();
  }

  // =========================================================
  // DELETE SUMMARY
  // DELETE: api/Summaries/62
  // =========================================================

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(
      int id)
  {
    var userId = GetUserId();

    Console.WriteLine(
        "=========================================="
    );

    Console.WriteLine(
        "DELETE SUMMARY"
    );

    Console.WriteLine(
        $"SummaryId: {id}"
    );

    Console.WriteLine(
        $"UserId: {userId}"
    );

    Console.WriteLine(
        "=========================================="
    );

    try
    {
      await _mediator.Send(
          new DeleteSummaryCommand
          {
            SummaryId = id,
            UserId = userId
          }
      );

      await _progressService.UpdateProgressAsync(
          userId
      );

      Console.WriteLine(
          $"SUMMARY DELETED: {id}"
      );

      return NoContent();
    }
    catch (KeyNotFoundException)
    {
      Console.WriteLine(
          $"SUMMARY NOT FOUND FOR DELETE: {id}"
      );

      return NotFound(
          new
          {
            message =
                  $"Summary with ID {id} was not found."
          }
      );
    }
  }

  // =========================================================
  // PDF UPLOAD
  // POST: api/Summaries/upload
  // =========================================================

  [HttpPost("upload")]
  public async Task<IActionResult> UploadPdf(
      IFormFile file,
      [FromForm] string title,
      [FromForm] string summaryStyle)
  {
    var userId = GetUserId();

    if (file == null || file.Length == 0)
    {
      return BadRequest(
          new
          {
            message = "No file uploaded."
          }
      );
    }

    var uploadsFolder =
        Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            "Summaries"
        );

    Directory.CreateDirectory(
        uploadsFolder
    );

    var savedFileName =
        $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

    var filePath =
        Path.Combine(
            uploadsFolder,
            savedFileName
        );

    await using (
        var stream =
            new FileStream(
                filePath,
                FileMode.Create
            ))
    {
      await file.CopyToAsync(stream);
    }

    string extractedText;

    using (
        var document =
            PdfDocument.Open(filePath))
    {
      var builder =
          new StringBuilder();

      foreach (
          var page
          in document.GetPages())
      {
        builder.AppendLine(
            page.Text
        );
      }

      extractedText =
          builder.ToString();
    }

    if (string.IsNullOrWhiteSpace(
        extractedText))
    {
      return BadRequest(
          new
          {
            message =
                  "No text found inside PDF."
          }
      );
    }

    // =====================================================
    // AI PROMPT
    // =====================================================

    var prompt = $@"
You are an AI Study Assistant.

Read the following study material.

Return ONLY valid JSON.

{{
  ""summary"": ""Write a clear plain text summary."",
  ""keywords"": [
    ""keyword1"",
    ""keyword2"",
    ""keyword3"",
    ""keyword4"",
    ""keyword5""
  ],
  ""questions"": [
    ""Question 1?"",
    ""Question 2?"",
    ""Question 3?""
  ]
}}

Do NOT make summary an object.
The summary must be a plain text string.

Study Material:

{extractedText}
";

    var aiResponse =
        await _ollamaService
            .GenerateResponseAsync(
                prompt
            );

    var start =
        aiResponse.IndexOf("{");

    var end =
        aiResponse.LastIndexOf("}");

    if (start >= 0 &&
        end > start)
    {
      aiResponse =
          aiResponse.Substring(
              start,
              end - start + 1
          );
    }

    var summary = "";
    var keywords = "[]";
    var questions = "[]";

    try
    {
      var json =
          JsonSerializer.Deserialize<JsonElement>(
              aiResponse
          );

      if (json.TryGetProperty(
          "summary",
          out var summaryElement))
      {
        summary =
            summaryElement.GetString()
            ?? "";
      }

      if (json.TryGetProperty(
          "keywords",
          out var keywordElement))
      {
        keywords =
            keywordElement.GetRawText();
      }

      if (json.TryGetProperty(
          "questions",
          out var questionElement))
      {
        questions =
            questionElement.GetRawText();
      }
    }
    catch
    {
      summary =
          aiResponse;
    }

    var command =
        new UploadSummaryCommand
        {
          Title = title,
          SummaryStyle = summaryStyle,

          FileName =
                file.FileName,

          FilePath =
                filePath,

          OriginalText =
                extractedText,

          SummaryText =
                summary,

          Keywords =
                keywords,

          Questions =
                questions,

          UserId =
                userId
        };

    var result =
        await _mediator.Send(
            command
        );

    await _progressService.UpdateProgressAsync(
        userId
    );

    return Ok(result);
  }
}
