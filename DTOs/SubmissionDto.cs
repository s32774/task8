namespace task8.DTOs;

public class SubmissionDto
{
    public int SubmissionId { get; set; }
    public string Student { get; set; } = string.Empty;
    public string Assignment { get; set; } = string.Empty;
    public string RepositoryUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? Score { get; set; }
    public string? Feedback { get; set; }
}