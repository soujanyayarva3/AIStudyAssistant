using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIStudyAssistant.API.Models;

public class StudyPlan
{
    [Key]
    public int PlanId { get; set; }

    public string TaskName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = "Pending";

    public int UserId { get; set; }


    [NotMapped]
    public string Title
    {
        get => TaskName;
        set => TaskName = value;
    }

    [NotMapped]
    public DateTime TargetDate
    {
        get => DueDate;
        set => DueDate = value;
    }
}