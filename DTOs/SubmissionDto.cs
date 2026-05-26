namespace UniversityTasksDbFirstApi.DTOs;

public class SubmissionDto
{
    public int SubmissionId { get; set; }
    public string StudentName { get; set; }
    public string AssignmentTitle { get; set; }
    public string RepositoryUrl { get; set; }
    public string Status { get; set; }
    public decimal? Score { get; set; }
    public string Feedback { get; set; }
}