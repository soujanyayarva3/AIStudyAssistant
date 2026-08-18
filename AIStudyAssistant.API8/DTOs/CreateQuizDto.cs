namespace AIStudyAssistant.Application.DTOs;

public class CreateQuizDto
{
    public string Title { get; set; } = string.Empty;

    public string Question { get; set; } = string.Empty;

    public string OptionA { get; set; } = string.Empty;

    public string OptionB { get; set; } = string.Empty;

    public string OptionC { get; set; } = string.Empty;

    public string OptionD { get; set; } = string.Empty;

    public string CorrectAnswer { get; set; } = string.Empty;

    public int Score { get; set; }
}