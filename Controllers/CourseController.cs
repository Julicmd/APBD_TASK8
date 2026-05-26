using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Models;

namespace UniversityTasksDbFirstApi.Controllers;

[ApiController]
[Route("api/courses")]
public class CourseController: ControllerBase
{
    private readonly UniversityTasksDbContext _dbContext;
    
    public  CourseController(UniversityTasksDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //Method Endpoint Required logic GET /api/courses?activeOnly=true
    //Return courses with assignment counts. Use AsNoTracking()and projection to DTO
    [HttpGet]
    public async Task<IActionResult> GetAllActiveCourses([FromQuery] bool activeOnly = false)
    {
        var course = await _dbContext.Courses.AsNoTracking()
            .Include(c=>c.Assignments)
            .Where(c => !activeOnly || c.IsActive)
            .ToListAsync();


        var result = course.Select(coa => new CourseDto
        {
            courseId = coa.CourseId,
            code = coa.Code,
            name = coa.Name,
            credits = coa.Credits,
            countAssignments =  coa.Assignments.Count
        });
        
        return Ok(result);
    }
    
    //GET /api/courses/{idCourse}/assignments?publishedOnly=true Return assignments for one course.
    //Include submission count. Return 404 if the course does not exist.
    [HttpGet("{idCourse:int}/assignments")]
    public async Task<IActionResult> GetAssignments(int idCourse,[FromQuery] bool publishedOnly = false)
    {
        var assigments = await _dbContext.Assignments
            .AsNoTracking()
            .Include(s => s.Submissions)
            .Where(a => a.CourseId == idCourse)
            .Where(a => !publishedOnly || a.IsPublished)
            .ToListAsync();

        var course = await _dbContext.Courses.AsNoTracking()
            .FirstOrDefaultAsync(a => a.CourseId == idCourse);

        if (course == null)
        {
            return NotFound();
        }

        var result = assigments.Select(a => new AssignmentDto
        {
            assignmentId = a.AssignmentId,
            title = a.Title,
            dueDate = a.DueDate,
            maxPoints = a.MaxPoints,
            isPublished = a.IsPublished,
            submissionCount = a.Submissions.Count
        });

        return Ok(result);
    }
    
}