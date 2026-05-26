namespace UniversityTasksDbFirstApi.DTOs;

public class AssignmentDto
{
    /// assignment id, title, due date, max points, published status, submission count
    public int assignmentId { get; set; }
    public string title { get; set; }
    public DateTime dueDate { get; set; }
    public int maxPoints { get; set; }
    public bool isPublished { get; set; }
    public int submissionCount { get; set; }
}