using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task8.Data;
using task8.DTOs;

namespace task8.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly UniversityTasksDbContext _context;

    public StudentsController(UniversityTasksDbContext context)
    {
        _context = context;
    }

    [HttpGet("{idStudent:int}/dashboard")]
    public async Task<IActionResult> GetStudentDashboard(int idStudent)
    {
        var student = await _context.Students
            .AsNoTracking()
            .Where(s => s.StudentId == idStudent)
            .Select(s => new StudentDashboardDto
            {
                StudentId = s.StudentId,
                IndexNumber = s.IndexNumber,
                FullName = s.FirstName + " " + s.LastName,
                IsActive = s.IsActive,

                Enrollments = s.Enrollments
                    .Select(e => e.Course.Code + " - " + e.Course.Name + " (" + e.Status + ")")
                    .ToList(),

                Submissions = s.Submissions
                    .Select(sub => new SubmissionDto
                    {
                        SubmissionId = sub.SubmissionId,
                        Student = s.FirstName + " " + s.LastName,
                        Assignment = sub.Assignment.Title,
                        RepositoryUrl = sub.RepositoryUrl,
                        Status = sub.Status,
                        Score = sub.Score,
                        Feedback = sub.Feedback
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (student is null)
        {
            return NotFound($"Student with id {idStudent} was not found.");
        }

        return Ok(student);
    }
}