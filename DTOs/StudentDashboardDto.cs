namespace UniversityTasksDbFirstApi.DTOs;

public class StudentDashboardDto
{
    //student id, index number, full name, active status, enrollments, submissions,
    
    public int studentId { get; set; }
    public string indexNum { get; set; } = null!;
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public List<EnrollmentDto> Enrollments { get; set; }
    public List<SubmissionDto> Submissions { get; set; }
}