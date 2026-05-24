using Microsoft.EntityFrameworkCore;
using task8.Data;
using task8.DTOs;
using task8.Exceptions;
using task8.Models;

namespace task8.Services;

public class SubmissionService : ISubmissionService
{
    private readonly UniversityTasksDbContext _context;

    public SubmissionService(UniversityTasksDbContext context)
    {
        _context = context;
    }

    public async Task<SubmissionDto> CreateSubmissionAsync(CreateSubmissionDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RepositoryUrl) ||
            !request.RepositoryUrl.StartsWith("https://"))
        {
            throw new BadRequestException("Repository URL must not be blank and must start with https://.");
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.StudentId == request.StudentId);

        if (student is null)
        {
            throw new NotFoundException($"Student with id {request.StudentId} was not found.");
        }

        if (!student.IsActive)
        {
            throw new BadRequestException("Student is not active.");
        }

        var assignment = await _context.Assignments
            .Include(a => a.Course)
            .FirstOrDefaultAsync(a => a.AssignmentId == request.AssignmentId);

        if (assignment is null)
        {
            throw new NotFoundException($"Assignment with id {request.AssignmentId} was not found.");
        }

        if (!assignment.IsPublished)
        {
            throw new BadRequestException("Assignment is not published.");
        }

        var isEnrolled = await _context.Enrollments.AnyAsync(e =>
            e.StudentId == request.StudentId &&
            e.CourseId == assignment.CourseId &&
            (e.Status == "Active" || e.Status == "Completed"));

        if (!isEnrolled)
        {
            throw new BadRequestException("Student is not enrolled in this course.");
        }

        var alreadySubmitted = await _context.Submissions.AnyAsync(s =>
            s.StudentId == request.StudentId &&
            s.AssignmentId == request.AssignmentId);

        if (alreadySubmitted)
        {
            throw new BadRequestException("Student already submitted this assignment.");
        }

        var now = DateTime.Now;

        var submission = new Submission
        {
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentId,
            RepositoryUrl = request.RepositoryUrl,
            SubmittedAt = now,
            Status = assignment.IsOverdue(now) ? "Late" : "Submitted"
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync();

        return new SubmissionDto
        {
            SubmissionId = submission.SubmissionId,
            Student = student.FullName,
            Assignment = assignment.Title,
            RepositoryUrl = submission.RepositoryUrl,
            Status = submission.Status,
            Score = submission.Score,
            Feedback = submission.Feedback
        };
    }

    public async Task GradeSubmissionAsync(int idSubmission, GradeSubmissionDto request)
    {
        var submission = await _context.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.SubmissionId == idSubmission);

        if (submission is null)
        {
            throw new NotFoundException($"Submission with id {idSubmission} was not found.");
        }

        if (request.Score < 0)
        {
            throw new BadRequestException("Score cannot be lower than 0.");
        }

        if (request.Score > submission.Assignment.MaxPoints)
        {
            throw new BadRequestException("Score cannot be higher than assignment max points.");
        }

        submission.Score = request.Score;
        submission.Feedback = request.Feedback;
        submission.Status = "Graded";

        await _context.SaveChangesAsync();
    }

    public async Task DeleteSubmissionAsync(int idSubmission)
    {
        var submission = await _context.Submissions
            .FirstOrDefaultAsync(s => s.SubmissionId == idSubmission);

        if (submission is null)
        {
            throw new NotFoundException($"Submission with id {idSubmission} was not found.");
        }

        if (submission.Status == "Graded")
        {
            throw new BadRequestException("Graded submission cannot be deleted.");
        }

        _context.Submissions.Remove(submission);
        await _context.SaveChangesAsync();
    }
}