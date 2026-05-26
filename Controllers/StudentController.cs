using System.Data.SqlTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/students")]
public class StudentController : ControllerBase
{
    private readonly UniversityTasksDbContext _dbContext;

    public StudentController(UniversityTasksDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    //GET /api/students/{idStudent}/dashboard Return student details, enrollments, and submissions.
    //Avoid the N+1 problem by using Include, ThenInclude, or projection.
    [HttpGet("{idStudent:int}/dashboard")]
    public async Task<IActionResult> GetDashboard(int idStudent)
    {
        var student = await _dbContext.Students.AsNoTracking()
            .Include(en => en.Enrollments)
            .ThenInclude(cr => cr.Course)
            .Include(sub => sub.Submissions)
            .ThenInclude(asi => asi.Assignment)
            .FirstOrDefaultAsync(st => st.StudentId == idStudent);

        if (student == null)
        {
            return NotFound();
        }

        var enrollment = student.Enrollments.Select(en => new EnrollmentDto
        {
            CourseId = en.CourseId,
            CourseName = en.Course.Name,
            Status = en.Status
        }).ToList();

        var submission = student.Submissions.Select(sub => new SubmissionDto
        {
            SubmissionId = sub.SubmissionId,
            StudentName = student.FullName,
            AssignmentTitle = sub.Assignment.Title,
            RepositoryUrl =  sub.RepositoryUrl,
            Status =  sub.Status,
            Score =  sub.Score,
            Feedback =  sub.Feedback

        }).ToList();

        var result = new StudentDashboardDto
        {
            studentId = student.StudentId,
            indexNum = student.IndexNumber,
            FullName = student.FullName,
            IsActive = student.IsActive,
            Enrollments = enrollment,
            Submissions = submission
        };

        return Ok(result);
    }
    

}