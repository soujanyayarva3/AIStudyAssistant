using AIStudyAssistant.Application.Interfaces.Repositories;
using AIStudyAssistant.Domain.Entities;
using AIStudyAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIStudyAssistant.Infrastructure.Repositories;

public class StudyPlanRepository : IStudyPlanRepository
{
    private readonly ApplicationDbContext _context;

    public StudyPlanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudyPlan>> GetStudyPlansAsync(int userId)
    {
        return await _context.StudyPlans
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<StudyPlan?> GetStudyPlanByIdAsync(int planId, int userId)
    {
        return await _context.StudyPlans
            .FirstOrDefaultAsync(x => x.PlanId == planId && x.UserId == userId);
    }

    public async Task<StudyPlan> CreateStudyPlanAsync(StudyPlan studyPlan)
    {
        _context.StudyPlans.Add(studyPlan);
        await _context.SaveChangesAsync();

        return studyPlan;
    }

    public async Task UpdateStudyPlanAsync(StudyPlan studyPlan)
    {
        _context.StudyPlans.Update(studyPlan);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteStudyPlanAsync(StudyPlan studyPlan)
    {
        _context.StudyPlans.Remove(studyPlan);
        await _context.SaveChangesAsync();
    }
}