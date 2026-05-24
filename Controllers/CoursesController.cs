using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task8.Data;
using task8.DTOs;

namespace task8.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly UniversityTasksDbContext _context;

    public CoursesController(UniversityTasksDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses([FromQuery] bool activeOnly = true)
    {
        var courses = await _context.Courses
            .AsNoTracking()
            .Where(c => !activeOnly || c.IsActive)
            .Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Code = c.Code,
                Name = c.Name,
                Credits = c.Credits,
                AssignmentCount = c.Assignments.Count
            })
            .ToListAsync();

        return Ok(courses);
    }

    [HttpGet("{idCourse:int}/assignments")]
    public async Task<IActionResult> GetAssignments(
        int idCourse,
        [FromQuery] bool publishedOnly = true)
    {
        var courseExists = await _context.Courses
            .AsNoTracking()
            .AnyAsync(c => c.CourseId == idCourse);

        if (!courseExists)
        {
            return NotFound($"Course with id {idCourse} was not found.");
        }

        var assignments = await _context.Assignments
            .AsNoTracking()
            .Where(a => a.CourseId == idCourse)
            .Where(a => !publishedOnly || a.IsPublished)
            .Select(a => new AssignmentDto
            {
                AssignmentId = a.AssignmentId,
                Title = a.Title,
                DueDate = a.DueDate,
                MaxPoints = a.MaxPoints,
                IsPublished = a.IsPublished,
                SubmissionCount = a.Submissions.Count
            })
            .ToListAsync();

        return Ok(assignments);
    }
}