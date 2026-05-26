namespace UniversityTasksDbFirstApi.DTOs;

public class CourseDto
{
    //- course id, code, name, credits, assignment count,
    public int courseId { get; set; }
    public string code{ get; set; }
    public string name { get; set; }
    public int credits { get; set; }
    public int countAssignments { get; set; }
    
}