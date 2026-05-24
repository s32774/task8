using task8.DTOs;

namespace task8.Services;

public interface ISubmissionService
{
    Task<SubmissionDto> CreateSubmissionAsync(CreateSubmissionDto request);
    Task GradeSubmissionAsync(int idSubmission, GradeSubmissionDto request);
    Task DeleteSubmissionAsync(int idSubmission);
}